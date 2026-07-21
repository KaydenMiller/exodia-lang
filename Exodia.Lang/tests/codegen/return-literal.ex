fn main(): int32 {
    mut x = 1;
    
    while (x < 10) {
        x = x + 1;
    }
    
    if (x == 10) {
        x = x + 5;
    }
    
    if (x > 10) {
        x = x + 1;
    }
    
    return x;
}