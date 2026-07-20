// match.ex -- exercises every branch of the match grammar that currently parses.
//
// Grammar shape being tested:
//   match_expression : MATCH expression '{' match_arm (',' match_arm)* ','? '}'
//   match_arm        : pattern (WHEN expression)? '=>' arm_body
//   arm_body         : expression | block_statement
//   pattern          : primary_pattern ('|' primary_pattern)*
//   primary_pattern  : qualified_name pattern_payload? | literal ('..' literal)? | '_'
//
// Scope notes (things intentionally NOT here because they don't parse yet):
//   - arm bodies are expression|block only -- `=> return x` is rejected; use `{ ...; }`
//   - a block arm is `{ statement* }`; each line needs its `;` (trailing
//     expression-as-value is a semantic feature not yet wired)
//   - arms are comma-separated -- the comma is required even after a `}` block arm

// --- variant destructuring + payload binding, expression-body arms ---
fn describe(r: Result<int32, String>): int32 {
    return match r {
        Ok(n)    => n,
        Err(msg) => 0,
    };
}

// --- payload-less variants + the wildcard `_` as catch-all ---
fn statusRank(s: Status): int32 {
    return match s {
        Active   => 2,
        Inactive => 1,
        _        => 0,
    };
}

// --- literal patterns + OR patterns (`|`) + ranges (`..`) ---
fn classify(code: int32): String {
    return match code {
        200 | 201 | 204 => "success",
        301 | 302       => "redirect",
        400..499        => "client error",
        500..599        => "server error",
        _               => "unknown",
    };
}

// --- `when` guards; a guarded arm doesn't satisfy exhaustiveness, so `_` remains ---
fn sign(n: int32): int32 {
    return match n {
        v when v > 0 => 1,
        v when v < 0 => 0,
        _            => 0,
    };
}

// --- block-body arm: do work across multiple statements (comma still required after `}`) ---
fn handle(r: Result<int32, String>): int32 {
    return match r {
        Ok(value) => value,
        Err(detail) => {
            Logging::Error(detail);
            Metrics::Increment("errors");
        },
    };
}

// --- multi-field payloads + qualified variant name in a pattern ---
fn area(s: Shape): double {
    return match s {
        Geometry::Empty => 0.0,
        Circle(radius)  => radius,
        Rect(w, h)      => w,
    };
}

// --- wildcard INSIDE a payload (`Some(_)`) ---
fn hasValue(opt: Option<int32>): int32 {
    return match opt {
        Some(_) => 1,
        None    => 0,
    };
}

// --- nested match: the arm body is itself a match expression ---
fn toStatus(r: Result<int32, ApiError>): int32 {
    return match r {
        Ok(v) => v,
        Err(e) => match e {
            NotFound          => 404,
            BadInput(msg)     => 400,
            Validation(items) => 422,
            _                 => 500,
        },
    };
}

// --- match used as a VALUE (const initializer) and as a bare expression-statement ---
fn useSites(r: Result<int32, String>): int32 {
    const mapped = match r {
        Ok(v)  => v,
        Err(_) => 0,
    };

    match r {
        Ok(_)  => Logging::Info("ok"),
        Err(_) => Logging::Warn("err"),
    };

    return mapped;
}

// --- match inside an `if` condition (the "match in an if" question) ---
fn guardWith(r: Result<int32, String>): int32 {
    if (match r { Ok(_) => true, Err(_) => false, }) {
        return 1;
    }
    return 0;
}
