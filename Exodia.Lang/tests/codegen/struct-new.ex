// struct-new.ex -- construct a struct value via positional new (field read comes next).
struct Point { x: int32; y: int32; }

fn main(): int32 {
    const p = new Point(3, 4);
    return 0;
}
