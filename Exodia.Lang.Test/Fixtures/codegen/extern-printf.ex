// extern-printf.ex -- FFI to libc's printf through an `extern fn` declaration.
//
// How it works, in three steps:
//   1. `extern fn` emits an LLVM `declare` (a signature with NO body). The compiler
//      does NOT define printf -- it just records that a symbol named `printf` exists
//      somewhere and will be supplied by the linker.
//   2. A string literal lowers to a private global byte array (with a trailing \00),
//      and its value is a `cstr` -- i.e. an i8* pointing at those bytes -- which is
//      exactly what printf's format argument expects.
//   3. At link time, `clang server.o` pulls libc in, and the unresolved `@printf`
//      symbol binds to the real libc printf. No runtime, no glue -- just the C ABI.
//
// IR this produces:
//   @str = private constant [.. x i8] c"linked to libc printf\0A\00"
//   declare i32 @printf(ptr)
//   ... call i32 @printf(ptr @str)
//
// The test harness only verifies the IR is well-formed (the `declare` + call are
// valid). To see it actually run, compile + link + execute:
//   dotnet run --project Exodia.Lang -- Fixtures/codegen/extern-printf.ex \
//     | llc -relocation-model=pic -filetype=obj -o /tmp/p.o - \
//     && clang /tmp/p.o -o /tmp/p && /tmp/p

// `...` marks a C-style variadic: ONE declaration handles any trailing args.
// It lowers to `declare i32 @printf(ptr, ...)`; each call passes however many args
// it likes, and LLVM emits the correct variadic call (right down to the x86-64 `al`
// register for float args).
extern fn printf(format: cstr, ...): int32;

fn main(): int32 {
    printf("linked to libc printf\n");
    printf("with args: %d and %f\n", 42, 3.14);
    return 0;
}
