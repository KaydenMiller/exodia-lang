// panic.ex -- `panic` as a diverging expression (type `never`).
//
//   expression       : assignment_expression | panic_expression ;
//   panic_expression : PANIC expression ;
//
// `panic` (renamed from `throw` to signal it is NOT the C-style exception you
// reach for by default -- it's the unrecoverable escape hatch) is a diverging
// EXPRESSION, so it can appear as a match arm body and an expression-bodied
// function body, not just as a statement.
// NOTE: `panic <expr>` accepts ANY expression syntactically; "the operand must
// be an error/exception type" is a TYPE-CHECKER rule, not the parser's job.

// --- statement position (via expression_statement) ---
fn alarm(): int32 {
    panic MakeError("boom");
}

// --- expression-bodied function body ---
fn always(): int32 => panic MakeError("nope");

// --- inside a block, guarded by an if ---
fn checked(x: int32): int32 {
    if (x < 0) {
        panic new ValueError("must be non-negative");
    }
    return x;
}

// --- as a match ARM body: one arm yields a value, the other diverges ---
fn mustParse(r: Result<uint16, String>): uint16 =>
    match r {
        Ok(value)  => value,
        Err(error) => panic new ParseException(error),
    };

// --- operand is a full expression: low precedence -> `panic (a + b)` ---
fn sumOrDie(a: int32, b: int32): int32 => panic a + b;

// --- operand forms: call, `new`, member access, and a bare string
//     (the string parses but is a semantic error -- documented above) ---
fn variants(): int32 {
    panic MakeError("call form");
    panic new Exception("new form");
    panic Errors::OutOfRange;
    panic "bare string parses, type-checks to an error";
}
