// print.ex -- print built-in across types (int32, double, int64, bool).
fn circleArea(r: double): double {
    const pi = 3.14159;
    return pi * r * r;
}

fn main(): int32 {
    print(42);                  // %d  -> 42
    print(circleArea(1.25));    // %f  -> 4.908734
    print(5000000000i64);       // %ld -> 5000000000
    print(1 < 2);               // bool -> zext -> %d -> 1
    return 0;
}
