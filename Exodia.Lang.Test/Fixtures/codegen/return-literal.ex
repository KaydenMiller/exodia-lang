fn main(): int32 {
    const cir = new Circle(5);
    const r2 = cir.r * cir.r;
    print(cir.area());
    const rect = new Rect(2, 3);
    print(rect.area());
    return 0;
}

interface IAreaCalc {
    area(): float;
}

struct Circle {
    r: int32;    
    ctor() {}
    
    ctor(r: int32) {
        this.r = r;
    }
}
impl IAreaCalc for Circle {
    area(): float {
        return ((this.r * this.r) as float) * 3.1415f;
    }
}

struct Rect {
    x: int32;
    y: int32;
    
    ctor(x: int32, y: int32) {
        this.x = x;
        this.y = y;
    }
}
impl IAreaCalc for Rect {
    area(): float {
        return (this.x * this.y) as float;
    }
}