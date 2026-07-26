// namespaces (codegen) -- members are flattened to qualified names at lowering.
//   - qualified references resolve for free (the ref text already spells the prefix)
//   - unqualified SIBLING calls resolve via the enclosing-namespace fallback
//   - namespaced structs/enums/methods all carry the qualified name
namespace Math {
    fn dbl(n: int32): int32 { return n * 2; }
    fn quad(n: int32): int32 { return dbl(dbl(n)); }   // unqualified sibling call

    struct Point {
        x: int32;
        y: int32;
        ctor(a: int32, b: int32) { this.x = a; this.y = b; }
        sum(): int32 { return this.x + this.y; }
    }

    enum Sign { Pos, Neg }
}

fn main(): int32 {
    const p = new Math::Point(3, 4);          // qualified new
    const s = Math::Sign::Pos;                 // namespaced enum construction
    const t = match s { Pos => 1, Neg => 0 };
    return Math::quad(p.sum()) + t;            // Math::quad(7)=28, +1 => 29
}
