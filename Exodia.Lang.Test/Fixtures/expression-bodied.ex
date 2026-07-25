// expression-bodied.ex -- expression-bodied functions & methods (C# `=> expr;`).
//
//   function_body : block_statement | FATARROW expression SEMI ;
//
// `=> expr` is the implicit return. Block bodies ALWAYS use explicit `return`
// (Exodia rejects Rust's trailing-expression-is-the-value rule). The `=>` here
// carries the same "evaluates to / returns" meaning as in match arms.

// --- free function, arithmetic body ---
fn add(a: int32, b: int32): int32 => a + b;

// --- body is a call ---
fn shout(text: String): String => Format(text);

// --- body is a match (the whole point: `=> match { arms }`, arms `=>` yield) ---
fn classify(code: int32): String =>
    match code {
        200      => "OK",
        404      => "Not Found",
        500..599 => "Server Error",
        _        => "Unknown",
    };

// --- body is an array literal ---
fn origin(): int32[] => [0, 0];

// --- block-bodied function STILL requires explicit return (both forms coexist) ---
fn describe(x: int32): String {
    if (x < 0) {
        return "negative";
    }
    return "non-negative";
}

struct Rect {
    private w: int32;
    private h: int32;

    public ctor(w: int32, h: int32) {
        this.w = w;
        this.h = h;
    }

    // --- expression-bodied METHOD on a value object ---
    public Area(): int32 => this.w * this.h;
}

class Greeter {
    // --- expression-bodied method whose body is a member access ---
    public Name(): String => this.storedName;

    private storedName: String;

    // --- and a block-bodied method alongside, for contrast ---
    public Greet(who: String): String {
        return who;
    }
}
