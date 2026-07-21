// return-literal.ex -- exercises casts (float->int, int widen/narrow) and unary negation.
fn circleArea(r: double): double {
    const pi = 3.14159;
    return pi * r * r;          // double arithmetic
}

fn main(): int32 {
    const a = circleArea(3.0);  // 3.14159 * 9 = 28.274...
    const area = a as int32;    // fptosi, truncates toward zero -> 28
    const n = 10 + -3;          // unary neg via BuildNeg -> 7
    const wide = area as int64; // sext i32 -> i64
    return (wide as int32) + n; // trunc back -> 28, + 7 -> 35
}
