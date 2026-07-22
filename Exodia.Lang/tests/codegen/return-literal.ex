fn main(): int32 {
    const a = circleArea(1.25d);
    print(a);
    print(12);
    return 0;
}

fn circleArea(r: double): double {
    const pi = 3.14159;
    return pi * r * r;          // double arithmetic
}