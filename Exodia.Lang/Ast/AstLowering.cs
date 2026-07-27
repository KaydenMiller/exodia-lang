using System.Globalization;
using Antlr4.Runtime;
using LLVMSharp.Interop;

namespace Exodia.Lang.Ast;

public class AstLowering
{
    private readonly ExpressionLowering _expressions = new();
    
    internal static TypeRef LowerType(ExodiaParser.TypeContext context)
    {
        TypeRef baseType =
            context.DYN() is not null ? new DynType(context.qualified_name().GetText(), Span(context))
            : context.type_arguments() is { } targs ? new GenericType(context.qualified_name().GetText(), targs.type().Select(LowerType).ToList(), Span(context))
            : new NamedType(context.qualified_name().GetText(), Span(context));
        // wrap once per `[]` suffix (direct '[' children, not the ones inside type_arguments): T[] , T[][]
        var depth = context.children?.Count(c => c is Antlr4.Runtime.Tree.ITerminalNode { } t && t.GetText() == "[") ?? 0;
        for (var i = 0; i < depth; i++)
            baseType = new ArrayType(baseType, Span(context));
        return baseType;
    }

    internal static TextSpan Span(ParserRuleContext context)
        => new(context.Start.StartIndex, context.Stop.StopIndex - context.Start.StartIndex + 1);

    private sealed record Collected(
        List<FnDeclaration> Functions, List<StructDeclaration> Structs,
        List<InterfaceDeclaration> Interfaces, List<ImplDeclaration> Impls, List<EnumDeclaration> Enums);

    private static string Qualify(string prefix, string name) => prefix == "" ? name : $"{prefix}::{name}";

    public ProgramNode LowerProgram(ExodiaParser.ProgramContext context)
    {
        var c = new Collected([], [], [], [], []);
        foreach (var statement in context.statement())
        {
            if (statement.function_declaration() is {} fn)
                c.Functions.Add(LowerFunction(fn));
            else if (statement.struct_declaration() is {} s)
                c.Structs.Add(LowerStruct(s));
            else if (statement.interface_declaration() is {} i)
                c.Interfaces.Add(LowerInterface(i));
            else if (statement.impl_declaration() is {} impl)
                c.Impls.Add(LowerImpl(impl));
            else if (statement.enum_declaration() is {} e)
                c.Enums.Add(LowerEnum(e));
            else if (statement.class_declaration() is {} cls)
                c.Structs.Add(LowerClass(cls));
            else if (statement.namespace_declaration() is {} ns)
                LowerNamespace(ns, "", c);
        }

        return new ProgramNode(c.Functions, c.Structs, c.Interfaces, c.Impls, c.Enums, Span(context));
    }

    // Flatten a namespace into the program's declaration lists, prefixing each member's name with
    // the namespace path. A qualified reference already spells that prefix, so refs resolve for free.
    private void LowerNamespace(ExodiaParser.Namespace_declarationContext ns, string parentPrefix, Collected c)
    {
        var prefix = Qualify(parentPrefix, ns.qualified_name().GetText());
        foreach (var m in ns.namespace_member())
        {
            if (m.function_declaration() is {} fn)
            {
                var d = LowerFunction(fn);
                // extern keeps its raw C symbol as LinkName; the Exodia name gets namespaced (§19)
                c.Functions.Add(d with { Name = Qualify(prefix, d.Name), LinkName = d.IsExtern ? (d.LinkName ?? d.Name) : d.LinkName });
            }
            else if (m.struct_declaration() is {} s)
            {
                var d = LowerStruct(s);
                c.Structs.Add(d with { Name = Qualify(prefix, d.Name) });
            }
            else if (m.enum_declaration() is {} e)
            {
                var d = LowerEnum(e);
                c.Enums.Add(d with { Name = Qualify(prefix, d.Name) });
            }
            else if (m.interface_declaration() is {} i)
            {
                var d = LowerInterface(i);
                c.Interfaces.Add(d with { Name = Qualify(prefix, d.Name) });
            }
            else if (m.impl_declaration() is {} impl)
            {
                var d = LowerImpl(impl);   // optimistic: assumes trait + target are in this namespace
                c.Impls.Add(d with { InterfaceName = Qualify(prefix, d.InterfaceName), TargetType = Qualify(prefix, d.TargetType) });
            }
            else if (m.class_declaration() is {} cls)
            {
                var d = LowerClass(cls);
                c.Structs.Add(d with { Name = Qualify(prefix, d.Name) });
            }
            else if (m.namespace_declaration() is {} nested)
                LowerNamespace(nested, prefix, c);
        }
    }

    private EnumDeclaration LowerEnum(ExodiaParser.Enum_declarationContext context)
    {
        var variants = (context.enum_variant_list()?.enum_variant() ?? [])
            .Select(v => new EnumVariant(
                v.identifier().GetText(),
                (v.enum_variant_payload()?.type() ?? []).Select(LowerType).ToList(),
                Span(v)))
            .ToList();
        return new EnumDeclaration(
            context.identifier().GetText(),
            LowerTypeParams(context.type_parameters(), context.where_clause()),
            variants,
            Span(context));
    }

    private MethodDeclaration LowerMethodDeclaration(ExodiaParser.Method_declarationContext m)
        => new(m.identifier().GetText(),
            LowerTypeParams(m.type_parameters(), m.where_clause()),
            LowerParams(m.formal_parameter_list()),
            LowerType(m.type()),
            LowerBody(m.function_body()),
            Span(m));

    private InterfaceDeclaration LowerInterface(ExodiaParser.Interface_declarationContext context)
    {
        var methods = context.interface_member()
            .Select(member => member.method_signature())
            .Select(s => new MethodSignature(
                s.identifier().GetText(),
                LowerTypeParams(context.type_parameters(), context.where_clause()),
                LowerParams(s.formal_parameter_list()),
                LowerType(s.type()),
                Span(s)))
            .ToList();
        return new InterfaceDeclaration(
            context.identifier().GetText(),
            LowerTypeParams(context.type_parameters(), context.where_clause()),
            methods, 
            Span(context));
    }

    private ImplDeclaration LowerImpl(ExodiaParser.Impl_declarationContext context)
        // increment A: no impl_outputs, so type(0)=interface, type(1)=target
        => new(context.type(0).qualified_name().GetText(),
               context.type(1).qualified_name().GetText(),
               LowerTypeParams(context.type_parameters(), context.where_clause()),
               context.method_declaration().Select(LowerMethodDeclaration).ToList(),
               Span(context));

    private static List<Param> LowerParams(ExodiaParser.Formal_parameter_listContext? list)
        => (list?.formal_parameter() ?? [])
            .Select(f => new Param(f.identifier().GetText(), LowerType(f.type()), Span(f)))
            .ToList();

    private IReadOnlyList<TypeParam> LowerTypeParams(
        ExodiaParser.Type_parametersContext? typeParams,
        ExodiaParser.Where_clauseContext[] whereClauses)
    {
        if (typeParams is null)
            return [];
        
        // name -> accumulating bound list; `order` preserves declaration order
        var bounds = new Dictionary<string, List<string>>();
        var order = new List<ExodiaParser.Type_parameterContext>();
        foreach (var typeParam in typeParams.type_parameter())
        {
            var list = new List<string>();
            if (typeParam.type() is { } inline)     // inline: `<T: IShape>`
                list.Add(inline.qualified_name().GetText());
            bounds[typeParam.identifier().GetText()] = list;
            order.Add(typeParam);
        }
        
        // fold `where T: A, B` into the matching param's bounds
        foreach (var clause in whereClauses)
        {
            if (!bounds.TryGetValue(clause.identifier().GetText(), out var list))
                continue;
            foreach (var type in clause.type())
                list.Add(type.qualified_name().GetText());
        }

        return order
            .Select(typeParam => new TypeParam(
                typeParam.identifier().GetText(),
                bounds[typeParam.identifier().GetText()],
                Span(typeParam)
            ))
            .ToList();
    }

    private FnDeclaration LowerFunction(ExodiaParser.Function_declarationContext context)
    {
        var isExtern = context.EXTERN() is not null;
        if (isExtern && context.type_parameters() is not null)
            throw new NotSupportedException("extern functions cannot be generic (no single C symbol)");
        var isVariadic = context.formal_parameter_list()?.ELLIPSIS() is not null;   // trailing `, ...`
        return new(context.identifier().GetText(),
            LowerTypeParams(context.type_parameters(), context.where_clause()),
            LowerParams(context.formal_parameter_list()),
            LowerType(context.type()),
            isExtern ? null : LowerBody(context.function_body()),
            isExtern,
            null,
            isVariadic,
            Span(context)
            );
    }

    private StructDeclaration LowerStruct(ExodiaParser.Struct_declarationContext context)
        => LowerTypeBody(context.member(), context.identifier().GetText(),
            LowerTypeParams(context.type_parameters(), context.where_clause()), false, Span(context));

    // class shares struct's member shape; the differences (heap, refcount, reference semantics) are codegen.
    private StructDeclaration LowerClass(ExodiaParser.Class_declarationContext context)
        => LowerTypeBody(context.member(), context.identifier().GetText(),
            LowerTypeParams(context.type_parameters(), context.where_clause()), true, Span(context));

    private StructDeclaration LowerTypeBody(ExodiaParser.MemberContext[] members, string name,
        IReadOnlyList<TypeParam> typeParams, bool isClass, TextSpan span)
    {
        var fields = new List<FieldDeclaration>();
        var constructors = new List<ConstructorDeclaration>();
        var methods = new List<MethodDeclaration>();
        foreach (var member in members)
        {
            var kind = member.member_kind();
            if (kind.field_declaration() is {} f)
                fields.Add(new FieldDeclaration(f.identifier().GetText(), LowerType(f.type()), Span(f)));
            else if (kind.constructor_declaration() is {} c)
                constructors.Add(new ConstructorDeclaration(
                    c.identifier()?.GetText(), LowerParams(c.formal_parameter_list()), LowerBlockStatement(c.block_statement()), Span(c)));
            else if (kind.method_declaration() is {} m)
                methods.Add(LowerMethodDeclaration(m));   // reads m.type_parameters(), not the struct's
        }
        return new StructDeclaration(name, typeParams, fields, constructors, methods, isClass, span);
    }

    private VariableDeclaration LowerVariableDeclaration(ExodiaParser.Variable_statementContext context)
    {
        var declaration = context.variable_declaration_list().variable_declaration();
        var initializer = declaration.variable_initializer()
                          ?? throw new NotSupportedException("variable without initializer not lowered yet");
        return new VariableDeclaration(
            declaration.identifier().GetText(),
            context.MUT() is not null,
            declaration.type() is {} t ? LowerType(t) : null,
            _expressions.Visit(initializer.assignment_expression()),
            Span(context));
    }

    private Block LowerBody(ExodiaParser.Function_bodyContext context)
    {
        if (context.block_statement() is { } block)
            return LowerBlockStatement(block);
        // `=> expr` desugars to `{ return expr; }` -- codegen only ever sees blocks.
        var span = Span(context);
        return new Block([new ReturnStatement(_expressions.Visit(context.expression()), span)], span);
    }

    private Block LowerBlockStatement(ExodiaParser.Block_statementContext context)
    {
        var statements = new List<Statement>();
        foreach (var statement in context.statement())
            statements.Add(LowerStatement(statement));
        return new Block(statements, Span(context));
    }

    private Statement LowerStatement(ExodiaParser.StatementContext context)
    {
        if (context.return_statement() is { } ret)
            return LowerReturn(ret);
        if (context.if_statement() is { } ifstat)
            return LowerIf(ifstat);
        if (context.block_statement() is { } block)
            return LowerBlockStatement(block);
        if (context.variable_statement() is { } var)
            return LowerVariableDeclaration(var);
        if (context.iteration_statement() is { } iter)
            return LowerIteration(iter);
        if (context.expression_statement() is { } expr)
            return new ExpressionStatement(_expressions.Visit(expr.expression()), Span(expr));
        throw new NotSupportedException($"statement not lowered yet: {context.GetText()}");
    }

    private ReturnStatement LowerReturn(ExodiaParser.Return_statementContext context)
    {
        var value = context.expression() is { } expr 
            ? _expressions.Visit(expr) 
            : null;
        return new ReturnStatement(value, Span(context));
    }

    private IfStatement LowerIf(ExodiaParser.If_statementContext context)
    {
        var cond = _expressions.Visit(context.expression());
        var then = LowerStatement(context.statement(0));
        var @else = context.ELSE() is not null
            ? LowerStatement(context.statement(1))
            : null;
        return new IfStatement(cond, then, @else, Span(context));
    }

    private Statement LowerIteration(ExodiaParser.Iteration_statementContext context)
    {
        if (context.while_statement() is { } w)
            return new WhileStatement(
                _expressions.Visit(w.expression()),
                LowerStatement(w.statement()),
                Span(context));
        // do-while / for come later (both desugar to while)
        throw new NotSupportedException($"loop not lowered yet: {context.GetText()}");
    }
}

public class ExpressionLowering : ExodiaBaseVisitor<Expr>
{
    public override Expr VisitQualified_name(ExodiaParser.Qualified_nameContext context)
    {
        if (context.GetText() == "unit")                          // the Unit value keyword (§20)
            return new UnitLiteral(AstLowering.Span(context));
        return new NameRef(context.GetText(), AstLowering.Span(context));
    }

    // `( expr )` -- without this, default VisitChildren returns the throwaway `)` (null).
    public override Expr VisitParenthesized_expression(ExodiaParser.Parenthesized_expressionContext context)
        => Visit(context.expression());

    public override Expr VisitTrue_literal(ExodiaParser.True_literalContext context)
        => new BoolLiteral(true, AstLowering.Span(context));

    public override Expr VisitFalse_literal(ExodiaParser.False_literalContext context)
        => new BoolLiteral(false, AstLowering.Span(context));

    public override Expr VisitNumeric_literal(ExodiaParser.Numeric_literalContext context)
    {
        var raw = context.GetText().Replace("_", ""); // strip digit separators
        var span = AstLowering.Span(context);

        if (context.FLOAT() is not null)
        {
            var last = raw[^1];
            var hasSuffix = last is 'f' or 'd' or 'm';
            var digits = hasSuffix ? raw[..^1] : raw;
            // carry the type as a NamedType (null -> double default); codegen resolves it.
            TypeRef? type = last switch
            {
                'f' => new NamedType("float", span),
                'd' => new NamedType("double", span),
                'm' => throw new NotSupportedException($"Decimal literal not supported yet: '{context.GetText()}'"),
                _   => null
            };
            return new FloatLiteral(double.Parse(digits, NumberStyles.Float, CultureInfo.InvariantCulture), type, span);
        }

        // integer: digits are [0-9], so the first i/u marks the suffix start.
        var suffixIdx = raw.IndexOfAny(['i', 'u']);
        if (suffixIdx < 0)
            return new IntLiteral(ulong.Parse(raw), null, span);   // int32 default

        // suffix -> canonical type name: i64 -> int64, u32 -> uint32
        var typeName = raw[suffixIdx..].Replace("i", "int").Replace("u", "uint");
        return new IntLiteral(ulong.Parse(raw[..suffixIdx]), new NamedType(typeName, span), span);
    }

    public override Expr VisitAssignment_expression(ExodiaParser.Assignment_expressionContext context)
    {
        if (context.assignment_expression() is null)
            return Visit(context.logical_OR_expression());

        var op = context.assignment_operator().GetText();
        if (op is not "=")
            throw new NotSupportedException($"compound assignment '{op}' not lowered yet");

        var target = Visit(context.left_hand_side_expression());
        var value = Visit(context.assignment_expression());
        return new AssignExpr(target, value, AstLowering.Span(context));
    }

    public override Expr VisitLogical_OR_expression(ExodiaParser.Logical_OR_expressionContext context)
    {
        if (context.op is null)
            return Visit(context.logical_AND_expression());
        return new BinaryExpr(Visit(context.left), context.op.Text, Visit(context.right), AstLowering.Span(context));
    }

    public override Expr VisitLogical_AND_expression(ExodiaParser.Logical_AND_expressionContext context)
    {
        if (context.op is null)
            return Visit(context.equality_expression());
        return new BinaryExpr(Visit(context.left), context.op.Text, Visit(context.right), AstLowering.Span(context));
    }

    public override Expr VisitUnary_expression(ExodiaParser.Unary_expressionContext context)
    {
        if (context.op is null)
            return Visit(context.postfix_expression());
        return new UnaryExpr(context.op.Text, Visit(context.unary_expression()), AstLowering.Span(context));
    }

    public override Expr VisitAdditive_expression(ExodiaParser.Additive_expressionContext context)
    {
        if (context.op is null)
            return Visit(context.multiplicative_expression());
        return new BinaryExpr(Visit(context.left), context.op.Text, Visit(context.right), AstLowering.Span(context));
    }

    public override Expr VisitMultiplicative_expression(ExodiaParser.Multiplicative_expressionContext context)
    {
        if (context.op is null)
            return Visit(context.cast_expression());
        return new BinaryExpr(Visit(context.left), context.op.Text, Visit(context.right), AstLowering.Span(context));
    }

    public override Expr VisitEquality_expression(ExodiaParser.Equality_expressionContext context)
    {
        if (context.op is null)
            return Visit(context.relational_expression());
        return new BinaryExpr(Visit(context.left), context.op.Text, Visit(context.right), AstLowering.Span(context));
    }

    public override Expr VisitRelational_expression(ExodiaParser.Relational_expressionContext context)
    {
        if (context.op is null)
            return Visit(context.shift_expression());
        return new BinaryExpr(Visit(context.left), context.op.Text, Visit(context.right), AstLowering.Span(context));
    }

    public override Expr VisitCast_expression(ExodiaParser.Cast_expressionContext context)
    {
        var value = Visit(context.unary_expression());
        foreach (var typeCtx in context.type())
            value = new CastExpr(value, AstLowering.LowerType(typeCtx), AstLowering.Span(typeCtx));
        return value;
    }

    public override Expr VisitNew_expression(ExodiaParser.New_expressionContext context)
    {
        var args = ExodiaHelpers.CollectArgs(context.arguments().argument_list())
            .Select(a => Visit(a.assignment_expression()))
            .ToList();
        var typeArgs = context.type_arguments() is { } targs      // explicit new Box<int32>(...)
            ? targs.type().Select(AstLowering.LowerType).ToList()
            : new List<TypeRef>();
        return new NewExpr(
            context.qualified_name().GetText(),
            typeArgs,
            context.identifier()?.GetText(),          // named-ctor part (new T.Named(...))
            args,
            AstLowering.Span(context));
    }

    public override Expr VisitThis_expression(ExodiaParser.This_expressionContext context)
        => new ThisExpr(AstLowering.Span(context));

    public override Expr VisitEnum_construction(ExodiaParser.Enum_constructionContext context)
        => new EnumConstructExpr(
            context.qualified_name().GetText(),
            context.type_arguments().type().Select(AstLowering.LowerType).ToList(),
            context.identifier().GetText(),
            context.arguments() is { } a ? LowerArgs(a) : [],
            AstLowering.Span(context));

    public override Expr VisitMatch_expression(ExodiaParser.Match_expressionContext context)
    {
        var scrutinee = Visit(context.expression());
        var arms = context.match_arm().Select(LowerArm).ToList();
        return new MatchExpr(scrutinee, arms, AstLowering.Span(context));
    }

    private MatchArm LowerArm(ExodiaParser.Match_armContext arm)
    {
        var pattern = LowerPattern(arm.pattern());
        var guard = arm.WHEN() is not null ? Visit(arm.expression()) : null;
        var body = arm.arm_body();
        if (body.block_statement() is not null)
            throw new NotSupportedException("block-bodied match arms not supported yet");
        return new MatchArm(pattern, guard, Visit(body.expression()), null, AstLowering.Span(arm));
    }

    private Pattern LowerPattern(ExodiaParser.PatternContext context)
    {
        var prims = context.primary_pattern();
        if (prims.Length > 1)
            throw new NotSupportedException("or-patterns (a | b) not supported yet");
        return LowerPrimaryPattern(prims[0]);
    }

    private Pattern LowerPrimaryPattern(ExodiaParser.Primary_patternContext context)
    {
        var span = AstLowering.Span(context);
        if (context.GetText() == "_")                             // '_' also lexes as an identifier -- check text first
            return new WildcardPattern(span);
        if (context.qualified_name() is { } qn)
        {
            var name = qn.identifier().Last().GetText();          // Option::Some -> Some
            var binding = context.identifier()?.GetText();        // trailing name: `Red r` / `Some(x) s`
            var payload = context.pattern_payload();
            return payload is not null || binding is not null
                ? new VariantPattern(name, (payload?.pattern() ?? []).Select(LowerPattern).ToList(), binding, span)
                : new NamePattern(name, span);                    // None (variant) or x (binding) -- resolved at match time
        }
        throw new NotSupportedException($"literal patterns not supported yet: {context.GetText()}");
    }

    public override Expr VisitString_literal(ExodiaParser.String_literalContext context)
    {
        var raw = context.GetText();
        var inner = raw.Substring(1, raw.Length - 2); // drop surrounding quotes
        var sb = new System.Text.StringBuilder();
        for (var i = 0; i < inner.Length; i++)
            sb.Append(inner[i] == '\\' && i + 1 < inner.Length
                ? inner[++i] switch
                {
                    'n' => '\n',
                    'r' => '\r',
                    't' => '\t',
                    '0' => '\0',
                    var c => c
                }
                : inner[i]
            );
        return new StringLiteral(sb.ToString(), AstLowering.Span(context));
    }

    public override Expr VisitPostfix_expression(ExodiaParser.Postfix_expressionContext context)
    {
        var ops = context.postfix_op();
        if (ops.Length == 0)
            return Visit(context.primary_expression());

        // f(args) -- free function call
        if (ops.Length == 1 && ops[0].arguments() is { } argsCtx)
            return new CallExpr(
                context.primary_expression().GetText(),
                LowerArgs(argsCtx),
                AstLowering.Span(context));

        // p.field -- field read
        if (ops.Length == 1 && ops[0].identifier() is { } fieldId)
            return new FieldAccess(Visit(context.primary_expression()), fieldId.GetText(), AstLowering.Span(context));

        // p.method(args) -- method call
        if (ops.Length == 2 && ops[0].identifier() is { } methodId && ops[1].arguments() is { } methodArgs)
            return new MethodCall(Visit(context.primary_expression()), methodId.GetText(), LowerArgs(methodArgs), AstLowering.Span(context));

        // a[i] -- index
        if (ops.Length == 1 && ops[0].expression() is { } idx)
            return new IndexExpr(Visit(context.primary_expression()), Visit(idx), AstLowering.Span(context));

        throw new NotSupportedException($"postfix form not lowered yet: {context.GetText()}");
    }

    public override Expr VisitArray_literal(ExodiaParser.Array_literalContext context)
        => new ArrayLiteral(context.expression().Select(Visit).ToList(), AstLowering.Span(context));

    private List<Expr> LowerArgs(ExodiaParser.ArgumentsContext arguments)
        => ExodiaHelpers.CollectArgs(arguments.argument_list())
            .Select(arg => Visit(arg.assignment_expression()))
            .ToList();
}