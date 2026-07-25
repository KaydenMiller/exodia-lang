struct Vec {
    x: int32;
    y: int32;
    ctor(a: int32) { this.x = a; this.y = a; }
    ctor(a: int32, b: int32) { this.x = a; this.y = b; }
    ctor Origin() { this.x = 0; this.y = 0; }
    sum(): int32 => this.x + this.y;
    add(n: int32): int32 { return this.x + this.y + n; }
}
fn main(): int32 {
    const a = new Vec(5);          print(a.sum());   // 10
    const b = new Vec(3, 4);       print(b.sum());   // 7
    const o = new Vec.Origin();    print(o.sum());   // 0
    print(a.add(100));                                // 110
    mut c = new Vec(1, 1);
    c.x = 20;                       print(c.x);       // 20 (field write)
    return 0;
}
