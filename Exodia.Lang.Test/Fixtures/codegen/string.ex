// String (§ strings) -- an immutable, RC-managed reference type (a class in all but name).
//   layout: { i64 rc, i64 len, <UTF-8 bytes><NUL> }  (bytes inline; data at offset 16)
//   - "..." is a String (built inline: calloc + memcpy, rc=1)
//   - `+` concatenates (calloc + two memcpys), `==`/`!=` compare bytes (inline memcmp)
//   - `.length` is the byte length
//   - a String passed to a `cstr` FFI parameter is coerced to its (null-terminated) data pointer
//   - RC-managed like any class: retained/released, freed at rc 0. ASan-clean.
// Strings are pure native LLVM (over libc calloc/memcpy/memcmp) -- no runtime shim to link.
extern fn puts(s: cstr): int32;

fn main(): int32 {
    const greeting = "Hello, " + "Exodia!";   // concat -> a fresh String
    puts(greeting);                           // String -> cstr coercion (fixed param)

    const a = "abc";
    const b = "abc";
    const c = "xyz";
    if (a == b) { puts("a == b"); }
    if (a != c) { puts("a != c"); }

    const n = greeting.length;                // 14 ("Hello, Exodia!"); i64 -- no implicit int conversion yet,
    return 0;                                 //   so we don't return it directly (that's a semantic-pass concern)
}
