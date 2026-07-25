struct Point {
    x: int32;
    y: int32;
    ctor(a: int32) {
        this.x = a;
        this.y = a * 2;    // computed
    }
}
fn main(): int32 {
    const p = new Point(5);
    print(p.x);   // 5
    print(p.y);   // 10
    return 0;
}
