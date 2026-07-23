using LLVMSharp.Interop;

namespace Exodia.Lang;

// A name bound in the current scope. Slot is the pointer to load/store/GEP through --
// a local's alloca, a parameter's slot, or a method's `this` pointer -- and Type is the
// element type stored there (opaque pointers don't carry the pointee type).
public sealed record Symbol(
    LLVMValueRef Slot,
    LLVMTypeRef Type);
