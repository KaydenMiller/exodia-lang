fn main(): int32 {
    const big = 5000000000i64;      // > 2^31, only valid as i64
    print(big);                      // 5000000000 via %ld
    if (big > 4000000000i64) { print(1); } else { print(0); }
    return 0;
}
