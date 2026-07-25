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
    print(match c {                      // 2 -> tag compare only
        Red   => 1,
        Green => 2,
        Blue  => 3
    });

    const g = IntOption::Some(15);
    print(match g {                      // 5 -> guard true; guard-false would fall through
        Some(n) when n >= 10 => 5,
        Some(n)              => 1,
        None                 => 0
    });

    const w = IntOption::Some(9);
    return match w {                     // 9 -> `Some(x) v` binds payload x and whole value v
        Some(x) v => x,
        None      => 0
    };
}
