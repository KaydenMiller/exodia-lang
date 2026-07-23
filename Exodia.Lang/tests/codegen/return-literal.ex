fn main(): int32 {
    const point = new Point(4);
    print(point.x);
    const pnt = point.scale(5);
    print(pnt.x);
    return 0;
}

struct Point {
    x: int32;
    y: int32;
    
    ctor() {}
    
    ctor(a: int32) {
        this.x = a;
        this.y = a * 2;
    }
    
    scale(factor: int32): Point {
        return new Point(this.x * factor);
    }
}