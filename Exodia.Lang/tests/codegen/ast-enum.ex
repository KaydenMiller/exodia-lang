// concrete enums -- tagged unions (option A: { i32 tag, ...payload slots }) + match.
enum IntOption {
    Some(int32),
    None
}
enum Shape {
    Rect(int32, int32),   // multi-payload variant
    Unit                  // payload-less variant
}
enum Color { Red, Green, Blue }   // C-style (all payload-less)

fn main(): int32 {
    const a = IntOption::Some(42);
    print(match a {                      // 42 -> binds payload
        Some(x) => x,
        None    => 0
    });

    const b = IntOption::None;
    print(match b {                      // 7 -> wildcard fallback
        Some(x) => x,
        _       => 7
    });

    const s = Shape::Rect(6, 7);
    print(match s {                      // 42 -> multi-payload bind
        Rect(w, h) => w * h,
        Unit       => 0
    });

    const c = Color::Green;
    return match c {                     // 2 -> tag compare only
        Red   => 1,
        Green => 2,
        Blue  => 3
    };
}
