fn add(a: int32, b: int32): int32 
{
    mut c = a + b;
    c = c + 3;
    return c;
}

fn main(): int32 {
    const result = add(5, 5);
    return result;
}