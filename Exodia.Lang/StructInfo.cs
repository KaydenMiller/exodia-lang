using LLVMSharp.Interop;

namespace Exodia.Lang;

public sealed record StructInfo(
    LLVMTypeRef Type,
    IReadOnlyDictionary<string, StructInfoField> Fields);
    
public sealed record StructInfoField(
    uint Index,
    LLVMTypeRef Type
);