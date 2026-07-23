using LLVMSharp.Interop;

namespace Exodia.Lang;

// An emitted or declared LLVM function plus its signature. BuildCall2 needs the
// signature because a function value's own .TypeOf is only an opaque `ptr`. Shared
// by ordinary functions, constructors, and methods.
public sealed record Callable(
    LLVMValueRef Fn,
    LLVMTypeRef Signature);
