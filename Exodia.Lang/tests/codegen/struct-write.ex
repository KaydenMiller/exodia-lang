// struct-write.ex -- field write on a struct local (GEP + store).
struct Point { x: int32; y: int32; }

fn main(): int32 {
    mut p = new Point(3, 4);
    p.x = 10;           // field write
    p.y = p.y + 1;      // field read + write
    print(p.x);         // 10
    print(p.y);         // 5
    return 0;
}
