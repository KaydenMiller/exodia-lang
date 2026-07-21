using System.Diagnostics;
using System.Globalization;
using LLVMSharp.Interop;

namespace Exodia.Lang;

public class CodeGenVisitor : ExodiaBaseVisitor<LLVMValueRef>
{
    public readonly LLVMModuleRef Module;
    public readonly LLVMBuilderRef Builder;
    
    private readonly Dictionary<string, (LLVMValueRef Slot, LLVMTypeRef Type)> _symbols = [];
    private readonly Dictionary<string, (LLVMValueRef Fn, LLVMTypeRef Signature)> _functions = [];

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
        var paramTypes = formals
            .Select(f => ExodiaHelpers.MapType(f.type()))
            .ToArray();
        var returnType = ExodiaHelpers.MapType(context.type());
        var fnType = LLVMTypeRef.CreateFunction(returnType, paramTypes);
        var fn = Module.AddFunction(name, fnType);
        _functions[name] = (fn, fnType);
        
        var entry = fn.AppendBasicBlock("entry");
        Builder.PositionAtEnd(entry);
        
        _symbols.Clear(); // fresh scope per function

        for (var i = 0; i < formals.Length; i++)
        {
            var pName = formals[i].identifier().GetText();
            var pValue = fn.GetParam((uint)i);
            var pSlot = Builder.BuildAlloca(paramTypes[i], pName);
            Builder.BuildStore(pValue, pSlot);
            _symbols[pName] = (pSlot, paramTypes[i]);
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
        var text = context.GetText().Replace("_", ""); // strip digit separators

        if (context.FLOAT() is not null)
        {
            // real literal -> double default. Suffixes f/d/m deferred.
            if (!double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out var d))
                throw new NotSupportedException($"Float literal suffix not supported yet: '{context.GetText()}'");
            return LLVMValueRef.CreateConstReal(LLVMTypeRef.Double, d);
        }
        
        // integer literal -> int32 default. Suffixes i8/u32/... deferred.
        if (!ulong.TryParse(text, out var l))
            throw new NotSupportedException($"Integer literal suffix not supported yet: '{context.GetText()}'");
        return LLVMValueRef.CreateConstInt(LLVMTypeRef.Int32, l);
    }

    public override LLVMValueRef VisitAdditive_expression(ExodiaParser.Additive_expressionContext context)
    {
        if (context.op == null)
            return Visit(context.multiplicative_expression());

        var left = Visit(context.left);
        var right = Visit(context.right);
        var isFloat = ExodiaHelpers.IsFloat(left.TypeOf);
        return context.op.Text switch
        {
            "+" => isFloat 
                ? Builder.BuildFAdd(left, right, "fadd") 
                : Builder.BuildAdd(left, right, "add"),
            "-" => isFloat
                ? Builder.BuildFSub(left, right, "fsub")
                : Builder.BuildSub(left, right, "sub"),
            _ => throw new NotSupportedException($"Additive op '{context.op.Text}'")
        };
    }

    public override LLVMValueRef VisitMultiplicative_expression(ExodiaParser.Multiplicative_expressionContext context)
    {
        if (context.op == null)
            return Visit(context.cast_expression());

        var left = Visit(context.left);
        var right = Visit(context.right);
        var isFloat = ExodiaHelpers.IsFloat(left.TypeOf);
        return context.op.Text switch
        {
            "*" => isFloat 
                ? Builder.BuildFMul(left, right, "fmul") 
                : Builder.BuildMul(left, right, "mul"),
            "/" => isFloat
                ? Builder.BuildFDiv(left, right, "fdiv") 
                : Builder.BuildSDiv(left, right, "div"),
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
        var value = Visit(context.variable_initializer()); // must visit first to allow type inference
        var type = context.type() is { } t ? ExodiaHelpers.MapType(t) : value.TypeOf; // annotation, else infer
        var slot = Builder.BuildAlloca(type, id);
        Builder.BuildStore(value, slot);
        _symbols[id] = (slot, type);
        return value;
    }

    // a == b , a != b   -> i1
    public override LLVMValueRef VisitEquality_expression(ExodiaParser.Equality_expressionContext context)
    {
        if (context.op is null)
            return Visit(context.relational_expression());

        var left = Visit(context.left);
        var right = Visit(context.right);        
        var isFloat = ExodiaHelpers.IsFloat(left.TypeOf);

        if (isFloat)
        {
            var fpredicate = context.op.Text switch
            {
                "==" => LLVMRealPredicate.LLVMRealOEQ,
                "!=" => LLVMRealPredicate.LLVMRealONE,
                _ => throw new NotSupportedException($"Equality op '{context.op.Text}'")
            };
            return Builder.BuildFCmp(fpredicate, left, right, "fcmp");
        }

        var predicate = context.op.Text switch
        {
            "==" => LLVMIntPredicate.LLVMIntEQ,
            "!=" => LLVMIntPredicate.LLVMIntNE,
            _ => throw new NotSupportedException($"Equality op '{context.op.Text}'")
        };
        return Builder.BuildICmp(predicate, left, right, "cmp");
    }
    
    // a < b , a > b , a <= b , a >= b   -> i1  (signed comparisons)
    public override LLVMValueRef VisitRelational_expression(ExodiaParser.Relational_expressionContext context)
    {
        if (context.op == null)
            return Visit(context.shift_expression());

        var left = Visit(context.left);
        var right = Visit(context.right);
        var isFloat = ExodiaHelpers.IsFloat(left.TypeOf);

        if (isFloat)
        {
            var fpred = context.op.Text switch
            {
                "<"  => LLVMRealPredicate.LLVMRealOLT,   // Signed Less Than
                ">"  => LLVMRealPredicate.LLVMRealOGT,
                "<=" => LLVMRealPredicate.LLVMRealOLE,
                ">=" => LLVMRealPredicate.LLVMRealOGE,
                _ => throw new NotSupportedException($"Relational op '{context.op.Text}'")
            };
            return Builder.BuildFCmp(fpred, left, right, "fcmp");
        }
        
        var pred = context.op.Text switch
        {
            "<"  => LLVMIntPredicate.LLVMIntSLT,   // Signed Less Than
            ">"  => LLVMIntPredicate.LLVMIntSGT,
            "<=" => LLVMIntPredicate.LLVMIntSLE,
            ">=" => LLVMIntPredicate.LLVMIntSGE,
            _ => throw new NotSupportedException($"Relational op '{context.op.Text}'")
        };
        return Builder.BuildICmp(pred, left, right, "cmp");
    }

    public override LLVMValueRef VisitIf_statement(ExodiaParser.If_statementContext context)
    {
        var condition = Visit(context.expression()); // an i1 from a comparison
        var fn = Builder.InsertBlock.Parent;    // the function we're emitting into
        var hasElse = context.ELSE() is not null;

        var thenBasicBlock = fn.AppendBasicBlock("then");
        var elseBasicBlock = hasElse ? fn.AppendBasicBlock("else") : default;
        var mergeBasicBlock = fn.AppendBasicBlock("ifcont");
        
        // with no else, the false edge jumps straight to the merge block
        Builder.BuildCondBr(condition, thenBasicBlock, hasElse ? elseBasicBlock : mergeBasicBlock);
        
        // --- then arm ---
        Builder.PositionAtEnd(thenBasicBlock);
        Visit(context.statement(0));
        
        // Only branch to merge if this arm didn't already terminate. If the then-branch
        // ended in `return`, the block already has a `ret` -- adding a `br` after it would 
        // be a SECOND terminator which is invalid IR
        if (Builder.InsertBlock.Terminator.Handle == IntPtr.Zero)
            Builder.BuildBr(mergeBasicBlock);
        
        // --- else arm ---
        if (hasElse)
        {
            Builder.PositionAtEnd(elseBasicBlock);
            Visit(context.statement(1));
            if (Builder.InsertBlock.Terminator.Handle == IntPtr.Zero)
                Builder.BuildBr(mergeBasicBlock);
        }
        
        // subsequent statements continue in the merge block
        Builder.PositionAtEnd(mergeBasicBlock);
        return default;
    }

    public override LLVMValueRef VisitWhile_statement(ExodiaParser.While_statementContext context)
    {
        var fn = Builder.InsertBlock.Parent;

        var condBB = fn.AppendBasicBlock("while.cond");
        var bodyBB = fn.AppendBasicBlock("while.body");
        var exitBB = fn.AppendBasicBlock("while.exit");
        
        // enter the loop: jump to the condition test
        Builder.BuildBr(condBB);
        
        // --- cond: eval the test every iteration, branch in or out ---
        Builder.PositionAtEnd(condBB);
        var condition = Visit(context.expression());
        Builder.BuildCondBr(condition, bodyBB, exitBB);
        
        // --- body: run it, then jump BACK to cond ---
        Builder.PositionAtEnd(bodyBB);
        Visit(context.statement());
        if (Builder.InsertBlock.Terminator.Handle == IntPtr.Zero)
            Builder.BuildBr(condBB);
        
        // --- exit: code after the loop continues here ---
        Builder.PositionAtEnd(exitBB);
        return default;
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
        if (!_symbols.TryGetValue(name, out var sym))
            throw new NotSupportedException($"Assignment to unknown name '{name}'");

        Builder.BuildStore(value, sym.Slot);
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
            if (!_functions.TryGetValue(fnName, out var target))
                throw new NotSupportedException($"Unknown function '{fnName}'");

            // collect args (walk the left-recursive list), eval each
            var argCtxs = CollectArgs(argsCtx.argument_list());
            var args = new LLVMValueRef[argCtxs.Count];
            for (var i = 0; i < argCtxs.Count; i++)
                args[i] = Visit(argCtxs[i].assignment_expression());
            
            // target.Type is already the full function type from declaration -- use directly
            return Builder.BuildCall2(target.Signature, target.Fn, args, "call");
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
        if (_symbols.TryGetValue(name, out var sym))
            return Builder.BuildLoad2(sym.Type, sym.Slot, name);

        throw new NotSupportedException($"Unknown name '{name}'");
    }
}