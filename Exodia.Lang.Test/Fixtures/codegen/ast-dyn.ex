interface IShape { Area(): int32; }
struct Circle {
    r: int32;
    ctor(radius: int32) { this.r = radius; }
}
struct Square {
    s: int32;
    ctor(side: int32) { this.s = side; }
}
impl IShape for Circle {
    Area(): int32 { return this.r * this.r * 3; }   // ~pi*r^2 -> 75
}
impl IShape for Square {
    Area(): int32 { return this.s * this.s; }        // 16
}
fn main(): int32 {
    const c  = new Circle(5);
    const sq = new Square(4);
    const dc = c  as dyn IShape;
    const ds = sq as dyn IShape;
    print(dc.Area());    // Circle -> 75  (dynamic dispatch)
    print(ds.Area());    // Square -> 16  (same call site, different fn)
    return 0;
}
