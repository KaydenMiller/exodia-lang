interface IShape {
    Area(): int32;
}
struct Circle {
    r: int32;
    ctor(radius: int32) { this.r = radius; }
}
impl IShape for Circle {
    Area(): int32 { return this.r * this.r * 3; }   // ~pi*r^2 (int)
    scaled(k: int32): int32 => this.Area() * k;      // impl method calling another impl method
}
fn main(): int32 {
    const c = new Circle(5);
    print(c.Area());        // 75
    print(c.scaled(2));     // 150
    return 0;
}
