using LLVMSharp.Interop;

namespace Exodia.Lang;

// Shared helpers used by AST lowering + codegen.
internal static class ExodiaHelpers
{
    // Walk the left-recursive argument_list, restoring source order.
    public static List<ExodiaParser.ArgumentContext> CollectArgs(ExodiaParser.Argument_listContext? list)
    {
        var result = new List<ExodiaParser.ArgumentContext>();
        while (list is not null)
        {
            result.Insert(0, list.argument());
            list = list.argument_list();
        }
        return result;
    }

    public static bool IsFloat(LLVMTypeRef t) =>
        t.Kind is LLVMTypeKind.LLVMFloatTypeKind or LLVMTypeKind.LLVMDoubleTypeKind;
}
