using LLVMSharp.Interop;

namespace Exodia.Lang;

internal static class ExodiaHelpers
{
    public static LLVMTypeRef MapType(ExodiaParser.TypeContext context)
    {
        var name = context.qualified_name().GetText();
        return name switch
        {
            "int8"  or "uint8"  => LLVMTypeRef.Int8,
            "int16" or "uint16" => LLVMTypeRef.Int16,
            "int32" or "uint32" => LLVMTypeRef.Int32,
            "int64" or "uint64" => LLVMTypeRef.Int64,
            "bool"              => LLVMTypeRef.Int1,
            "char"              => LLVMTypeRef.Int32,   // Unicode scalar (Rust-style 32-bit) -- confirm
            "float"             => LLVMTypeRef.Float,    // 32-bit IEEE
            "double"            => LLVMTypeRef.Double,   // 64-bit IEEE
            "void"              => LLVMTypeRef.Void,
            _ => throw new NotSupportedException($"Type '{name}' not supported in codegen yet")
        };
    }
}