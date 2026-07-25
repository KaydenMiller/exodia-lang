using LLVMSharp.Interop;

namespace Exodia.Lang;

// An enum lowers to a "wide struct": { i32 tag, ...payload slots for every variant }.
// Kept in its OWN registry (never _structs) so the compiler always knows it is an enum,
// not a struct -- the hook a future reflection/metaprogramming pass hangs off.
public sealed record EnumInfo(
    LLVMTypeRef Type,
    IReadOnlyDictionary<string, EnumVariantInfo> Variants);

// One variant: its discriminant (declaration order) and where its payload lives in the wide struct.
// Reuses StructInfoField (Index, Type) -- a payload element IS a field at an index with a type.
public sealed record EnumVariantInfo(
    int Tag,
    IReadOnlyList<StructInfoField> Payload);
