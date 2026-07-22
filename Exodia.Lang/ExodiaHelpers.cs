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
            _ => throw new NotSupportedException($"Type '{name}' not supported in codegen")
        };
    }

    public static LLVMTypeRef MapIntSuffixType(string suffix)
    {
        return suffix switch
        {
            "i8" or "u8" => LLVMTypeRef.Int8,
            "i16" or "u16" => LLVMTypeRef.Int16,
            "i32" or "u32" => LLVMTypeRef.Int32,
            "i64" or "u64" => LLVMTypeRef.Int64,
            _ => throw new NotSupportedException($"Type suffix '{suffix}' not supported in codegen")
        };
    }

    public static LLVMTypeRef MapFloatSuffixType(char suffix)
    {
        return suffix switch
        {
            'f' => LLVMTypeRef.Float,
            'd' => LLVMTypeRef.Double,
            'm' => throw new NotImplementedException($"Decimal literal not supported yet: 'm'"),
            _ => LLVMTypeRef.Double, // no suffix -> double default
        };
    }
    
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