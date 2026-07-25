// generic structs -- inferred + explicit type args, multi-param, and nested (structural inference).
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
    ctor(b: Box<T>) { this.inner = b; }   // param `Box<T>` -> T inferred structurally from the arg
}
fn main(): int32 {
    const bi = new Box(7);            print(bi.get());     // 7  -> inferred Box$i32
    const bd = new Box<double>(2.5);                        //    -> explicit Box$double
    const p = new Pair(9, 1.5);       print(p.first());    // 9  -> Pair$i32$double
    const w = new Wrapper(bi);                              //    -> Wrapper$i32 (structural inference)
    const inner = w.inner;            print(inner.get());  // 7
    return 0;
}
