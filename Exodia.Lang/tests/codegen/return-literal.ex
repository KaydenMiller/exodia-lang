fn main(): int32 {
    mut x = 11;
    if (x < 10) {
        x = x + 100;
    } else {
        x = x + 1;
    }
    return x;
}