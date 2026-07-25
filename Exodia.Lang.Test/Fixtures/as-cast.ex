// as-cast.ex -- Rust-style postfix `as` value cast.
//
//   cast_expression : unary_expression (AS type)* ;
//
// Sits between unary and multiplicative in the precedence ladder: `as` binds
// TIGHTER than `*`/`+` but LOOSER than unary `-`. Semantics (later): a value
// conversion (`x as int32`) needs a declared/implicit conversion; an interface
// upcast (`x as dyn IShape`) is a non-lossy view. The grammar accepts any
// `expr as type` -- meaning is a semantic check.

interface IShape { Area(): double; }

fn casts(x: int32, s: String): int32 {
    // --- primitive conversions ---
    const a = x as int64;
    const b = x as double;
    const c = 3.5 as int32;

    // --- library type, array, generic, dyn (all valid `type`s after `as`) ---
    const d = s as String;
    const e = x as int32[];
    const f = x as List<int32>;
    const g = x as dyn IShape;

    // --- chained casts (parses; semantics decides) ---
    const h = x as int32 as int64;

    // --- precedence: `as` tighter than `*`/`+`, looser than unary `-` ---
    const i = x * x as int64;        // -> x * (x as int64)
    const j = x as int64 + 1;        // -> (x as int64) + 1
    const k = -x as int32;           // -> (-x) as int32

    return a as int32;               // cast in a return expression
}

// --- cast as a call argument ---
fn useArg(x: int32): int32 => takesLong(x as int64);

// --- cast in an if condition ---
fn cond(x: int32): int32 {
    if (x as int64 > 100) {
        return 1;
    }
    return 0;
}
