// control-flow.ex -- regression guard for the block-like statements:
// if / else-if / else, while, for, and their braced + brace-less body forms.
//
// These all work because a body is a `statement`, and `block_statement` is one
// alternative of `statement`. If someone later changes a body from `statement`
// to `block_statement` (mandating braces), or removes the recursive `else`,
// this file should start FAILING -- that's the point.

// --- if / else-if / else chain (else-if = `else` whose body is a nested `if`) ---
fn ifChain(x: int32): int32 {
    if (x < 0) {
        return 0;
    } else if (x == 0) {
        return 1;
    } else if (x < 10) {
        return 2;
    } else {
        return 3;
    }
}

// --- brace-less single-statement bodies, incl. brace-less if/else ---
fn bracelessBodies(x: int32): int32 {
    if (x < 0) return 0;
    if (x > 0) return 1; else return 2;
    return 3;
}

// --- nested if: dangling `else` must bind to the NEAREST if ---
fn nestedIf(a: int32, b: int32): int32 {
    if (a > 0)
        if (b > 0) return 1; else return 2;
    return 0;
}

// --- while: braced and brace-less ---
fn whileForms(n: int32): int32 {
    mut sum: int32 = 0;
    mut i:   int32 = 0;

    while (i < n) {
        sum += i;
        i   += 1;
    }

    while (sum > 100) sum -= 1;       // brace-less body

    return sum;
}

// --- for: braced (with compound && condition) and brace-less ---
fn forForms(n: int32): int32 {
    mut total: int32 = 0;

    for (mut i: int32 = 0; i < n && total < 1000; i += 1) {
        total += i;
    }

    for (mut j: int32 = 0; j < n; j += 1) total += 1;   // brace-less body

    return total;
}

// --- do-while: braced and brace-less body (grammar: `DO statement WHILE ( expr )`) ---
fn doWhileForms(n: int32): int32 {
    mut k: int32 = 0;

    do {
        k += 1;
    } while (k < n)

    return k;
}
