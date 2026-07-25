// generic free functions -- monomorphized per call-site type via argument inference.
fn id<T>(x: T): T { return x; }
fn pick<A, B>(a: A, b: B): A { return a; }
fn main(): int32 {
    print(id(42));         // 42  -> id$1$i32
    print(id(3));          // 3   -> reuses id$1$i32 (emit-once)
    print(pick(9, 2.5));   // 9   -> pick$2$i32$double
    return id(0);
}
