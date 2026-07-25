using LLVMSharp.Interop;

namespace Exodia.Lang.Ast;

public static class AstHelpers
{
    public static LLVMTypeRef MapPrimitiveType(LLVMContextRef ctx, string name) => name switch
    {
        "int8"  or "uint8"  => ctx.Int8Type,
        "int16" or "uint16" => ctx.Int16Type,
        "int32" or "uint32" => ctx.Int32Type,
        "int64" or "uint64" => ctx.Int64Type,
        "bool"              => ctx.Int1Type,
        "char"              => ctx.Int32Type,
        "float"             => ctx.FloatType,
        "double"            => ctx.DoubleType,
        "unit" or "void"    => ctx.VoidType,   // Unit is zero-sized -> LLVM void in return position (§20)
        "cstr"              => LLVMTypeRef.CreatePointer(ctx.Int8Type, 0),
        _ => throw new NotSupportedException($"Type '{name}' not supported in codegen")
    };
    
    public static LLVMIntPredicate IntPredicate(string op) => op switch
    {
        "<"  => LLVMIntPredicate.LLVMIntSLT,
        ">"  => LLVMIntPredicate.LLVMIntSGT,
        "<=" => LLVMIntPredicate.LLVMIntSLE,
        ">=" => LLVMIntPredicate.LLVMIntSGE,
        "==" => LLVMIntPredicate.LLVMIntEQ,
        "!=" => LLVMIntPredicate.LLVMIntNE,
        _ => throw new NotSupportedException($"int comparison '{op}'")
    };

    public static LLVMRealPredicate FloatPredicate(string op) => op switch
    {
        "<"  => LLVMRealPredicate.LLVMRealOLT,
        ">"  => LLVMRealPredicate.LLVMRealOGT,
        "<=" => LLVMRealPredicate.LLVMRealOLE,
        ">=" => LLVMRealPredicate.LLVMRealOGE,
        "==" => LLVMRealPredicate.LLVMRealOEQ,
        "!=" => LLVMRealPredicate.LLVMRealONE,
        _ => throw new NotSupportedException($"float comparison '{op}'")
    };

    public static LLVMValueRef EmitCast(this LLVMBuilderRef builder, LLVMValueRef value, LLVMTypeRef target)
    {
        var source = value.TypeOf;
        if (source.Handle == target.Handle) return value;
        var srcFloat = ExodiaHelpers.IsFloat(source);
        var dstFloat = ExodiaHelpers.IsFloat(target);
        if (!srcFloat && !dstFloat)
        {
            if (source.IntWidth == target.IntWidth) return value;
            if (source.IntWidth < target.IntWidth)
                return source.IntWidth == 1
                    ? builder.BuildZExt(value, target, "zext")
                    : builder.BuildSExt(value, target, "sext");
            return builder.BuildTrunc(value, target, "trunc");
        }
        if (!srcFloat && dstFloat) return builder.BuildSIToFP(value, target, "sitofp");
        if (srcFloat && !dstFloat) return builder.BuildFPToSI(value, target, "fptosi");
        var srcBits = source.Kind == LLVMTypeKind.LLVMDoubleTypeKind ? 64 : 32;
        var dstBits = target.Kind == LLVMTypeKind.LLVMDoubleTypeKind ? 64 : 32;
        if (srcBits == dstBits) return value;
        return srcBits < dstBits 
            ? builder.BuildFPExt(value, target, "fpext") 
            : builder.BuildFPTrunc(value, target, "fptrunc");
    }
}