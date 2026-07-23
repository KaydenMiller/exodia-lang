fn main(): int32 {
    const point = new Point(4, 3);
    print(point.x);
    print(point.y);
    point.y = 10;
    print(point.y);
    return 0;
}

struct Point {
    x: int32;
    y: int32;
}