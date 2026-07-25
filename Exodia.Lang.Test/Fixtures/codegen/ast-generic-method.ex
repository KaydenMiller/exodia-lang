// generic methods -- method-level <U> on both plain and generic structs; env composition (T + U).
struct Wrap {
    v: int32;
    ctor(a: int32) { this.v = a; }
    echo<U>(x: U): U { return x; }          // method-level generic on a non-generic struct
}
struct Box<T> {
    v: T;
    ctor(a: T) { this.v = a; }
    pickFirst<U>(other: U): T { return this.v; }   // struct T + method U in one body
}
fn main(): int32 {
    const w = new Wrap(0);
    print(w.echo(7));          // 7   -> Wrap.echo$i32
    const b = new Box<int32>(11);
    print(b.pickFirst(2.5));   // 11  -> Box$i32.pickFirst$double (T=i32, U=double)
    print(b.pickFirst(9));     // 11  -> Box$i32.pickFirst$i32
    return 0;
}
