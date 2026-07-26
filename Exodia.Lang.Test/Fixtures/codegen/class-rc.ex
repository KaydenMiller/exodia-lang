// class reference counting (§17 increment 2) -- retain/release inserted automatically.
//   - `new` = +1 (owned); a call returning a class = +1 (transferred to the receiver)
//   - `const b = a` (borrow) retains -> b is a second owner
//   - scope exit (block end / return / fall-off) releases owned class locals -> rc--, free at 0
//   - a RETURNED class local transfers its +1 to the caller (not released here)
//   validated ASan-clean (no leaks / double-free / use-after-free).
class Box {
    v: int32;
    ctor(x: int32) { this.v = x; }
    get(): int32 { return this.v; }
}

fn make(): Box { 
    const b = new Box(7); 
    return b; 
}   // transfer: b's +1 goes to the caller

fn main(): int32 {
    const a = new Box(10);              // +1
    const alias = a;                    // retain -> rc 2
    const fromFactory = make();         // +1 transferred in

    mut sum = 0;
    if (sum == 0) {
        const scoped = new Box(5);      // freed at this block's end
        sum = scoped.get();
    }

    return a.get() + alias.get() + fromFactory.get() + sum;   // 10 + 10 + 7 + 5 = 32
    // scope exit releases a (rc 2->1), alias (1->0 free), fromFactory (1->0 free)
}
