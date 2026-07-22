// struct-field.ex -- Slice 1 complete: register + construct + read fields.
struct Point { x: int32; y: int32; }

fn main(): int32 {
    const p = new Point(3, 4);
    print(p.x);   // 3
    print(p.y);   // 4
    return 0;
}
