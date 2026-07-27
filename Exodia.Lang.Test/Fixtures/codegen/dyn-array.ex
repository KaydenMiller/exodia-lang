// Array of `dyn` (§23 x §16-B) -- a heterogeneous collection behind an interface.
//   `dyn HasArea` is a 16-byte fat pointer { data ptr, vtable ptr } (a VALUE, not a class),
//   so the array machinery monomorphizes it to `%Array$dyn.HasArea = { i64 rc, i64 len }`
//   with the fat pointers packed inline at offset 16 -- RefHeap=null (no per-element RC).
//   Verified: mixed Circle+Square in one array, each element vtable-dispatched -> 3 + 4 = 7,
//   ASan-clean.
//
// Three constraints today (all pre-existing dyn / lowering gaps, NOT array bugs):
//   1. The upcast source must be a NAMED LOCAL (`c as dyn HasArea`). An inline `new Circle(3)
//      as dyn HasArea` fails: a struct is a value type with no address for the fat pointer's
//      data slot, and a fresh `new` temp has no lvalue.
//   2. `shapes[0].area()` CHAINED is not lowered ("postfix form not lowered yet"). Break it
//      with an intermediate local: `const a = shapes[0]; a.area()`. (VisitPostfix gap, all arrays.)
//   3. IMPLICIT upcast via a `: dyn HasArea[]` annotation does not work -- the literal infers
//      its element type from the elements, ignoring the target type. Write `as dyn HasArea`.
//
// Lifetime caveat: the fat pointer holds a BORROWED data pointer (here, to stack structs).
// Fine within one frame; returning/storing the array beyond the source's lifetime would dangle.
interface HasArea { area(): int32; }

struct Circle { r: int32; ctor(r: int32) { this.r = r; } }
impl HasArea for Circle { area(): int32 => this.r; }

struct Square { s: int32; ctor(s: int32) { this.s = s; } }
impl HasArea for Square { area(): int32 => this.s; }

fn main(): int32 {
    const c = new Circle(3);
    const q = new Square(4);
    const shapes = [c as dyn HasArea, q as dyn HasArea];   // mixed types, one array (constraint 1)
    const first = shapes[0];                               // break the chain (constraint 2)
    const second = shapes[1];
    return first.area() + second.area();                    // 3 (Circle) + 4 (Square) = 7
}
