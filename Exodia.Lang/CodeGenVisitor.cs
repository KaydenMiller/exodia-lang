using LLVMSharp.Interop;

namespace Exodia.Lang;

public class CodeGenVisitor
{
    public readonly LLVMModuleRef Module;
    public readonly LLVMBuilderRef Builder;

    public CodeGenVisitor(LLVMModuleRef module)
    {
        Module = module;
        Builder = Module.Context.CreateBuilder();
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