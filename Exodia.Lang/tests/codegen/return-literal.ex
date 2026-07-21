fn main(): int32 {
    const big = 5000000000i64;   // > 2^31, only valid as i64
    if (big > 4000000000i64) {   // i64 comparison
        return 7;
    }
    return 0;
}