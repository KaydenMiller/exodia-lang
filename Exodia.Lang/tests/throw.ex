// throw.ex -- `throw` as a diverging expression (type `never`).
//
//   expression       : assignment_expression | throw_expression ;
//   throw_expression : THROW expression ;
//
// Being an EXPRESSION (not a statement) is what lets `throw` appear as a match
// arm body and an expression-bodied function body -- not just as a statement.
// NOTE: `throw <expr>` accepts ANY expression syntactically; "the operand must
// be an exception type" is a TYPE-CHECKER rule, not the parser's job. So
// `throw "bare string"` parses here but would be a semantic error later.

// --- statement position (via expression_statement) ---
fn alarm(): int32 {
    throw MakeError("boom");
}

// --- expression-bodied function body ---
fn always(): int32 => throw MakeError("nope");

// --- inside a block, guarded by an if ---
fn checked(x: int32): int32 {
    if (x < 0) {
        throw new ValueError("must be non-negative");
    }
    return x;
}

// --- as a match ARM body: one arm yields a value, the other diverges ---
fn mustParse(r: Result<uint16, String>): uint16 =>
    match r {
        Ok(value)  => value,
        Err(error) => throw new ParseException(error),
    };

// --- operand is a full expression: low precedence -> `throw (a + b)` ---
fn sumOrDie(a: int32, b: int32): int32 => throw a + b;

// --- operand forms: call, `new`, member access, and a bare string
//     (the string parses but is a semantic error -- documented above) ---
fn variants(): int32 {
    throw MakeError("call form");
    throw new Exception("new form");
    throw Errors::OutOfRange;
    throw "bare string parses, type-checks to an error";
}
