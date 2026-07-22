// two-pass.ex -- forward reference (main calls fns defined below) + mutual recursion.
fn main(): int32 {
    print(addLater(3, 4));   // forward ref -> 7
    print(isEven(10));       // mutual recursion -> 1
    print(isEven(7));        // -> 0
    return 0;
}

fn addLater(a: int32, b: int32): int32 {
    return a + b;
}

fn isEven(n: int32): int32 {
    if (n == 0) { return 1; }
    return isOdd(n - 1);     // isOdd defined AFTER isEven
}

fn isOdd(n: int32): int32 {
    if (n == 0) { return 0; }
    return isEven(n - 1);
}
