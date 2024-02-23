using LLVMSharp.Interop;

namespace Interperter.Kaleidoscope;

public class KaleidoscopeVisitor : KaleidoscopeBaseVisitor<int>
{
    private readonly LLVMModuleRef _module;
    private readonly LLVMBuilderRef _llvmBuilder;
    private readonly Dictionary<string, LLVMValueRef> _namedValues = new();
    private readonly Stack<LLVMValueRef> _valueStack = new();
    
    public override unsafe int VisitNumberExpression(KaleidoscopeParser.NumberExpressionContext context)
    {
        if (context.NUMBER() is { } num)
        {
            var value = double.Parse(num.GetText());
            _valueStack.Push(LLVM.ConstReal(LLVM.DoubleType(), value));
        }

        return 0;
    }

    public override int VisitVariableExpression(KaleidoscopeParser.VariableExpressionContext context)
    {
        var identifier = context.IDENTIFIER().GetText()!;

        if (_namedValues.TryGetValue(identifier, out var value))
        {
            _valueStack.Push(value);
        }
        else
        {
            throw new Exception("Unknown variable name");
        }

        return 0;
    }

    public override int VisitExpression(KaleidoscopeParser.ExpressionContext context)
    {
        Visit(context.primary());
        Visit(context.binaryExpression());
        return 0;
    }

    public override int VisitContinuedBinaryExpression(KaleidoscopeParser.ContinuedBinaryExpressionContext context)
    {
        return 0;
    }

    public override unsafe int VisitFinalBinaryExpression(KaleidoscopeParser.FinalBinaryExpressionContext context)
    {
        Visit(context.primary());

        var r = _valueStack.Pop();
        var l = _valueStack.Pop();

        LLVMValueRef result = context.OP().GetText() switch
        {
            "+" => LLVM.BuildFAdd(_llvmBuilder, l, r, "addtmp".ConvertToSByte()),
            "-" => LLVM.BuildFSub(_llvmBuilder, l, r, "subtmp".ConvertToSByte()),
            "*" => LLVM.BuildFMul(_llvmBuilder, l, r, "multmp".ConvertToSByte()),
            _ => throw new NotImplementedException()
        };
        
        _valueStack.Push(result);
        
        return 0;
    }

    public override unsafe int VisitCallExpression(KaleidoscopeParser.CallExpressionContext context)
    {
        var functionId = context.IDENTIFIER().GetText()!;
        var callee = LLVM.GetNamedFunction(_module, functionId.ConvertToSByte());

        if (callee is null)
        {
            throw new Exception("Unknown function referenced");
        }

        var parameterCount = context.expression().Length;
        if (LLVM.CountParams(callee) != parameterCount)
        {
            throw new Exception("Incorrect number of arguments passed to fn");
        }

        var argsV = new LLVMValueRef[parameterCount];
        for (var i = 0; i < parameterCount; i++)
        {
            Visit(context.expression()[i]);
            argsV[i] = _valueStack.Pop();
        }
        
        _valueStack.Push(LLVM.BuildCall2(
            _llvmBuilder,
            LLVM.DoubleType(),
            callee,
            argsV, 
            parameterCount,
            "calltmp".ConvertToSByte()));

        return 0;
    }

}