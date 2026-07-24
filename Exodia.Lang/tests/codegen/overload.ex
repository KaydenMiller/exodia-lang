// overload.ex -- stress the postfix/CallableKey paths: fn overloading, method calls
// with args, named + arity-overloaded ctors.
struct Vec {
    x: int32;
    y: int32;
    ctor(a: int32) { this.x = a; this.y = a; }              // arity 1
    ctor(a: int32, b: int32) { this.x = a; this.y = b; }    // arity 2
    ctor Origin() { this.x = 0; this.y = 0; }               // named
    sum(): int32 => this.x + this.y;                        // method, 0 args
    add(n: int32): int32 => this.x + this.y + n;            // method, 1 arg
}

fn scale(v: int32): int32 => v * 2;                          // fn overload, arity 1
fn scale(v: int32, by: int32): int32 => v * by;             // fn overload, arity 2

fn main(): int32 {
    const a = new Vec(5);          print(a.sum());   // 10
    const b = new Vec(3, 4);       print(b.sum());   // 7
    const o = new Vec.Origin();    print(o.sum());   // 0
    print(a.add(100));                                // 110
    print(scale(5));                                  // 10
    print(scale(5, 3));                               // 15
    return 0;
}
