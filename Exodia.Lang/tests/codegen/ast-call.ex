fn main(): int32 {
    return factorial(5);              // forward ref + call
}
fn factorial(n: int32): int32 {
    if (n <= 1) { return 1; }
    return n * factorial(n - 1);      // recursion
}
