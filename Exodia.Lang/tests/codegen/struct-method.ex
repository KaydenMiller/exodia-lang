struct Point {
    x: int32;
    y: int32;
    ctor(a: int32, b: int32) { this.x = a; this.y = b; }
    sum(): int32 => this.x + this.y;
    scaled(factor: int32): int32 { return (this.x + this.y) * factor; }
}
fn main(): int32 {
    const p = new Point(3, 4);
    print(p.sum());        // 7
    print(p.scaled(10));   // 70
    return 0;
}
