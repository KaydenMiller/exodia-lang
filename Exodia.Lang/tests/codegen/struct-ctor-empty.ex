struct Thing {
    v: int32;
    ctor() { }        // empty body -- v left uninitialized
}
fn main(): int32 {
    const t = new Thing();   // should compile + run (v is undef, we don't read it)
    print(42);
    return 0;
}
