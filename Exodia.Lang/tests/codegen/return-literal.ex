fn main(): int32 {
    mut x = 1.5;       // inferred double
    x = x + 2.0;       // fadd -> 3.5
    x = x * 2.0;       // fmul -> 7.0
    if (x > 7.5) {     // fcmp ogt -> false
        return 1;
    }
    if (x == 7.0) {    // fcmp oeq -> true
        return 42;
    }
    return 0;
}