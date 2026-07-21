using LLVMSharp.Interop;

namespace Exodia.Lang;

public class CodeGenVisitor : ExodiaBaseVisitor<LLVMValueRef>
{
    public readonly LLVMModuleRef Module;
    public readonly LLVMBuilderRef Builder;
    private readonly Dictionary<string, LLVMValueRef> _symbols = [];

    public CodeGenVisitor(LLVMModuleRef module)
    {
        Module = module;
        Builder = Module.Context.CreateBuilder();
    }
    
    // fn <name>(...): int32 { ... }   -- for now assume i32 return, no params
    public override LLVMValueRef VisitFunction_declaration(ExodiaParser.Function_declarationContext context)
    {
        var name = context.identifier().GetText(); // "main"
        
        // parameters -- all int32 for now
        var formals = context.formal_parameter_list()?.formal_parameter() ?? [];
        var paramTypes = new LLVMTypeRef[formals.Length];
        for (var i = 0; i < formals.Length; i++)
            paramTypes[i] = LLVMTypeRef.Int32;
        
        var fnType = LLVMTypeRef.CreateFunction(LLVMTypeRef.Int32, paramTypes);
        var fn = Module.AddFunction(name, fnType);
        
        var entry = fn.AppendBasicBlock("entry");
        Builder.PositionAtEnd(entry);
        
        _symbols.Clear(); // fresh scope per function

        for (var i = 0; i < formals.Length; i++)
        {
            var pName = formals[i].identifier().GetText();
            var pValue = fn.GetParam((uint)i);
            var slot = Builder.BuildAlloca(LLVMTypeRef.Int32, pName);
            Builder.BuildStore(pValue, slot);
            _symbols[pName] = slot;
        }

        Visit(context.function_body());
        return fn;
    }
    
    // return <expr>;
    public override LLVMValueRef VisitReturn_statement(ExodiaParser.Return_statementContext context)
    {
        var value = Visit(context.expression());
        return Builder.BuildRet(value);
    }
    
    // an integer literal, e.g. 0, 42, 69, 420
    public override LLVMValueRef VisitNumeric_literal(ExodiaParser.Numeric_literalContext context)
    {
        var value = ulong.Parse(context.GetText());
        return LLVMValueRef.CreateConstInt(LLVMTypeRef.Int32, value);
    }

    public override LLVMValueRef VisitAdditive_expression(ExodiaParser.Additive_expressionContext context)
    {
        if (context.op == null)
            return Visit(context.multiplicative_expression());

        var left = Visit(context.left);
        var right = Visit(context.right);
        return context.op.Text switch
        {
            "+" => Builder.BuildAdd(left, right, "add"),
            "-" => Builder.BuildSub(left, right, "sub"),
            _ => throw new NotSupportedException($"Additive op '{context.op.Text}'")
        };
    }

    public override LLVMValueRef VisitMultiplicative_expression(ExodiaParser.Multiplicative_expressionContext context)
    {
        if (context.op == null)
            return Visit(context.cast_expression());

        var left = Visit(context.left);
        var right = Visit(context.right);
        return context.op.Text switch
        {
            "*" => Builder.BuildMul(left, right, "mul"),
            "/" => Builder.BuildSDiv(left, right, "div"),
            _ => throw new NotSupportedException($"Multiplicative op '{context.op.Text}'")
        };
    }

    public override LLVMValueRef VisitParenthesized_expression(ExodiaParser.Parenthesized_expressionContext context)
    {
        return Visit(context.expression());
    }

    public override LLVMValueRef VisitVariable_statement(ExodiaParser.Variable_statementContext context)
        => Visit(context.variable_declaration_list());

    public override LLVMValueRef VisitVariable_declaration(ExodiaParser.Variable_declarationContext context)
    {
        var id = context.identifier().GetText();
        var type = context.type()?.GetText();
        var value = Visit(context.variable_initializer());
        var slot = Builder.BuildAlloca(LLVMTypeRef.Int32, id);
        Builder.BuildStore(value, slot);
        _symbols[id] = slot;
        return value;
    }

    // x = <expr> -- mutation of an existing local
    public override LLVMValueRef VisitAssignment_expression(ExodiaParser.Assignment_expressionContext context)
    {
        // passthrough alt: `assignment_expression :  logical_OR_expression`.
        // EVERY ordinary expression flows through here -- must be preserved.
        if (context.assignment_expression() is null)
            return Visit(context.logical_OR_expression());

        var op = context.assignment_operator().GetText();
        if (op is not "=")
            throw new NotSupportedException($"Compound assignment '{op}' not supported yet");
        
        // RHS first (visit right-recursive, so `a = b = 5` chains for free
        var value = Visit(context.assignment_expression());
        
        // Resolve the LHS to its EXISTING slot. Do NOT Visit() the LHS -- visiting a name
        // goes through VisitQualified_name -> BuildLoad2, which loads the value. We want the 
        // slot (address) to store INTO, not a load.
        var name = context.left_hand_side_expression().GetText();
        if (!_symbols.TryGetValue(name, out var slot))
            throw new NotSupportedException($"Assignment to unknown name '{name}'");

        Builder.BuildStore(value, slot);
        return value; // assignment yields the assigned value
    }

    public override LLVMValueRef VisitPostfix_expression(ExodiaParser.Postfix_expressionContext context)
    {
        var ops = context.postfix_op();

        if (ops.Length == 0)
            return Visit(context.primary_expression());

        if (ops.Length == 1 && ops[0].arguments() is { } argsCtx)
        {
            // the callee is resolved by NAME here -- NOT visited as a variable
            var fnName = context.primary_expression().GetText(); // "add"
            var callee = Module.GetNamedFunction(fnName);
            if (callee.Handle == IntPtr.Zero)
                throw new NotSupportedException($"Unknown function '{fnName}'");
            
            // collect args (walk the left-recursive list), eval each
            var argCtxs = CollectArgs(argsCtx.argument_list());
            var args = new LLVMValueRef[argCtxs.Count];
            for (var i = 0; i < argCtxs.Count; i++)
                args[i] = Visit(argCtxs[i].assignment_expression());
            
            // BuildCall2 needs the function TYPE (opaque pointers -> the call site states the signature)
            var paramTypes = new LLVMTypeRef[args.Length];
            Array.Fill(paramTypes, LLVMTypeRef.Int32);
            var fnType = LLVMTypeRef.CreateFunction(LLVMTypeRef.Int32, paramTypes);

            return Builder.BuildCall2(fnType, callee, args, "call");
        }

        throw new NotSupportedException("Only simple f(args) calls supported so far");

        static List<ExodiaParser.ArgumentContext> CollectArgs(ExodiaParser.Argument_listContext? list)
        {
            var result = new List<ExodiaParser.ArgumentContext>();
            while (list is not null)
            {
                result.Insert(0, list.argument());
                list = list.argument_list();
            }
            return result;
        }
    }

    public override LLVMValueRef VisitQualified_name(ExodiaParser.Qualified_nameContext context)
    {
        var name = context.GetText();
        if (_symbols.TryGetValue(name, out var slot))
            return Builder.BuildLoad2(LLVMTypeRef.Int32, slot, name);

        throw new NotSupportedException($"Unknown name '{name}'");
    }
}