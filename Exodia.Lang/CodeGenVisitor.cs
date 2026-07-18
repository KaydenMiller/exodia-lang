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
}