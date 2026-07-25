fn main(): int32 {
    mut r = 0;
    if ((3 < 5) && (10 > 2)) { r = r + 1; }       // true && true -> +1
    if ((5 > 10) && (1 < 2)) { r = r + 10; }      // short-circuit false -> skip
    if ((5 > 10) || (2 < 3)) { r = r + 100; }     // false || true -> +100
    return r;                                      // 101
}
