using LLVMSharp.Interop;

namespace Interperter.Kaleidoscope;

public unsafe class KaleidoscopeVisitor : KaleidoscopeBaseVisitor<int>
{
    private readonly LLVMOpaqueBuilder* _llvmBuilder;
    private readonly LLVMOpaqueModule* _module;
    private readonly Dictionary<string, LLVMOpaqueValue> _namedValues = new();
    private readonly Stack<LLVMOpaqueValue> _valueStack = new();

    public KaleidoscopeVisitor(LLVMOpaqueModule* module, LLVMOpaqueBuilder* builder)
    {
        _module = module;
        _llvmBuilder = builder;
    }

    public override int VisitNumberExpression(KaleidoscopeParser.NumberExpressionContext context)
    {
        Console.WriteLine("Parsing a numeric expression");
        
        if (context.NUMBER() is { } num)
        {
            var value = double.Parse(num.GetText());
            _valueStack.Push(*LLVM.ConstReal(LLVM.DoubleType(), value));
        }

        return 0;
    }

    public override int VisitVariableExpression(KaleidoscopeParser.VariableExpressionContext context)
    {
        Console.WriteLine("Parsing a variable expression");
        
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
        Console.WriteLine("Parsing an expression");
        
        Visit(context.primary());
        Visit(context.binaryExpression());
        return 0;
    }

    public override int VisitContinuedBinaryExpression(KaleidoscopeParser.ContinuedBinaryExpressionContext context)
    {
        Console.WriteLine("Parsing a binary expression");
        
        Visit(context.expression());
        return 0;
    }

    public override int VisitFinalBinaryExpression(KaleidoscopeParser.FinalBinaryExpressionContext context)
    {
        Console.WriteLine("Parsing a binary expression");
        
        Visit(context.primary());

        var r = _valueStack.Pop();
        var l = _valueStack.Pop();

        var result = context.OP().GetText() switch
        {
            "+" => *LLVM.BuildFAdd(_llvmBuilder, &l, &r, "addtmp".ToSByte()),
            "-" => *LLVM.BuildFSub(_llvmBuilder, &l, &r, "subtmp".ToSByte()),
            "*" => *LLVM.BuildFMul(_llvmBuilder, &l, &r, "multmp".ToSByte()),
            _ => throw new NotImplementedException()
        };

        _valueStack.Push(result);

        return 0;
    }

    public override int VisitCallExpression(KaleidoscopeParser.CallExpressionContext context)
    {
        Console.WriteLine("Parsing a call expression");
        
        var functionId = context.IDENTIFIER().GetText()!;
        var callee = LLVM.GetNamedFunction(_module, functionId.ToSByte());

        if (callee is null)
        {
            throw new Exception("Unknown function referenced");
        }

        var parameterCount = (uint)context.expression().Length;
        if (LLVM.CountParams(callee) != parameterCount)
        {
            throw new Exception("Incorrect number of arguments passed to fn");
        }

        var args = new LLVMOpaqueValue[parameterCount];
        for (var i = 0; i < parameterCount; i++)
        {
            Visit(context.expression()[i]);
            var arg = _valueStack.Pop();
            args[i] = arg;
        }

        var argptr = args.ToArrayPointer();

        var call = LLVM.BuildCall2(
            _llvmBuilder,
            LLVM.DoubleType(),
            callee,
            &argptr,
            parameterCount,
            "calltmp".ToSByte());

        _valueStack.Push(*call);

        return 0;
    }

    public override int VisitPrototype(KaleidoscopeParser.PrototypeContext context)
    {
        Console.WriteLine("Parsing a prototype");
        
        var argumentCount = (uint)context.IDENTIFIER().Length - 1;
        var arguments = new LLVMOpaqueType[argumentCount];
        var functionName = context.IDENTIFIER()[0].GetText()!;

        var function = LLVM.GetNamedFunction(_module, functionName.ToSByte());

        // If F conflicted, there was already something named 'Name'.
        if (function is not null)
        {
            if (LLVM.CountBasicBlocks(function) != 0)
            {
                throw new Exception("redefinition of function");
            }


            if (LLVM.CountParams(function) != argumentCount)
            {
                throw new Exception("redefinition of function with different number of args");
            }
        }
        else
        {
            for (var i = 0; i < argumentCount; i++) 
            {
                arguments[i] = *LLVM.DoubleType();
            }

            var args = arguments.ToArrayPointer();
            var fnType = LLVM.FunctionType(
                LLVM.DoubleType(),
                &args,
                argumentCount,
                0
            );

            function = LLVM.AddFunction(
                _module,
                functionName.ToSByte(),
                fnType
            );

            LLVM.SetLinkage(function, LLVMLinkage.LLVMExternalLinkage);
        }

        // for (var i = 0; i < argumentCount; i++)
        // {
        //     var identifiers = context.IDENTIFIER()[1..]!
        //        .Select(id => id.GetText())
        //        .ToArray();
        //
        //     var param = LLVM.GetParam(function, (uint)i);
        //     var name = identifiers[i];
        //     LLVM.SetValueName2(param, name.ToSByte(), (uint)name.Length);
        // }

        _valueStack.Push(*function);
        return 0;
    }

    public override int VisitDefinition(KaleidoscopeParser.DefinitionContext context)
    {
        Console.WriteLine("Parsing a Function Definition");
        
        Visit(context.prototype());
        var function = _valueStack.Pop();
        _namedValues.Clear();
        
        // create basic block to start insertion into
        LLVM.PositionBuilderAtEnd(_llvmBuilder, LLVM.AppendBasicBlock(&function, "entry".ToSByte()));

        try
        {
            Visit(context.expression());
        }
        catch (Exception)
        {
            LLVM.DeleteFunction(&function);
            throw;
        }

        // Finish off the function
        var retValue = _valueStack.Pop();
        LLVM.BuildRet(_llvmBuilder, &retValue);

        // Validate the generated code, check for consistency
        LLVM.VerifyFunction(&function, LLVMVerifierFailureAction.LLVMPrintMessageAction);

        return 0;
    }
}