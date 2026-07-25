fn main(): int32 {
    mut i = 1;
    mut sum = 0;
    while (i <= 5) { sum = sum + i; i = i + 1; }
    return sum;                                   // 1+2+3+4+5 = 15
}
