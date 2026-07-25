// give.ex -- `give` produces the value of the enclosing block/match arm.
//
//   give_statement : GIVE expression SEMI ;   (in `statement`)
//
// `give` is to a BLOCK/ARM what `return` is to a FUNCTION: it ends the block with
// a value. It's the explicit "produce a value" for a block arm (Exodia has no
// Rust block-tail). Distinct from the EXIT operators:
//   - return / ? / panic  -> leave the FUNCTION
//   - give                -> produce the enclosing MATCH ARM's value (execution
//                            continues after the match)
//
// Only meaningful inside a value-producing block/arm; that restriction is a
// type-checker rule, not the parser's job.

// --- give in a block arm, ASSIGNMENT position (the case `return` couldn't cover) ---
fn classify(x: int32): int32 {
    const label = match x {
        0 => 100,                                  // expression arm: implicit
        _ => {                                     // block arm: work THEN give
            Logging::Trace("nonzero");
            give 200;
        },
    };
    return label;
}

// --- give in a block arm, RETURN position (expression-bodied fn) ---
fn statusFor(code: int32): String =>
    match code {
        200 => "OK",
        500 => {
            Metrics::Increment("errors");
            give "Server Error";
        },
        _   => "Unknown",
    };

// --- mixed arms: expression, side-effect-only (unit), and give ---
fn handle(e: Event): int32 {
    const result = match e {
        Tick        => 1,                          // expression arm
        Log(message) => {                          // side-effect arm -> unit (no give)
            Logging::Info(message);
        },
        Compute(n)  => {                           // work then give a value
            const doubled = n * 2;
            give doubled;
        },
    };
    return result;
}

// --- give whose value is itself a call / expression ---
fn build(n: int32): int32 =>
    match n {
        _ => {
            prepare(n);
            give combine(n, n + 1);
        },
    };
