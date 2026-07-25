// generic structs -- explicit type args on `new`, multi-param, and nested (Box<T> inside Wrapper<T>).
struct Box<T> {
    v: T;
    ctor(a: T) { this.v = a; }
    get(): T { return this.v; }
}
struct Pair<A, B> {
    a: A;
    b: B;
    ctor(x: A, y: B) { this.a = x; this.b = y; }
    first(): A { return this.a; }
}
struct Wrapper<T> {
    inner: Box<T>;
    ctor(b: Box<T>) { this.inner = b; }   // field/param typed with the struct's own T
}
fn main(): int32 {
    const bi = new Box<int32>(7);            print(bi.get());     // 7  -> Box$i32
    const bd = new Box<double>(2.5);                                //    -> Box$double
    const p = new Pair<int32, double>(9, 1.5); print(p.first());  // 9  -> Pair$i32$double
    const w = new Wrapper<int32>(bi);                              //    -> Wrapper$i32
    const inner = w.inner;            print(inner.get());  // 7
    return 0;
}
