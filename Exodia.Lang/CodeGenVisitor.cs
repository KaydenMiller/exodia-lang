using LLVMSharp.Interop;

namespace Exodia.Lang;

public class CodeGenVisitor : ExodiaBaseVisitor<LLVMValueRef>
{
    public readonly LLVMModuleRef Module;
    public readonly LLVMBuilderRef Builder;

    public CodeGenVisitor(LLVMModuleRef module)
    {
        Module = module;
        Builder = Module.Context.CreateBuilder();
    }
    
    // fn <name>(...): int32 { ... }   -- for now assume i32 return, no params
    public override LLVMValueRef VisitFunction_declaration(ExodiaParser.Function_declarationContext context)
    {
        var name = context.identifier().GetText(); // "main"
        var fnType = LLVMTypeRef.CreateFunction(LLVMTypeRef.Int32, Array.Empty<LLVMTypeRef>());
        var fn = Module.AddFunction(name, fnType);

        var entry = fn.AppendBasicBlock("entry");
        Builder.PositionAtEnd(entry);

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


    public string EmitTrivialMain()
    {
        // the function type: i32 ()
        var fnType = LLVMTypeRef.CreateFunction(LLVMTypeRef.Int32, Array.Empty<LLVMTypeRef>());
        
        // the function in the module
        var main = Module.AddFunction("main", fnType);
        
        // entry block, point builder at it
        var entry = main.AppendBasicBlock("entry");
        Builder.PositionAtEnd(entry);
        
        // return 0;
        Builder.BuildRet(LLVMValueRef.CreateConstInt(LLVMTypeRef.Int32, 0));

        return Module.PrintToString();
    }
}