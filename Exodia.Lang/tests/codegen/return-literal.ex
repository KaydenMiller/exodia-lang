fn main(): int32 {
    const point = new Point(4);
    print(point.x);
    print(point.y);
    point.y = 10;
    print(point.y);
    
    const pt = new Point();
    pt.x = 1;
    pt.y = 2;
    print(pt.x);
    print(pt.y);
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
}