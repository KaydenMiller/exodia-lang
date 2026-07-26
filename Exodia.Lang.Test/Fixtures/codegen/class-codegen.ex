// class (increment 1) -- heap-allocated reference type (§17).
//   - layout: { i64 refcount, ...fields }, referenced by pointer
//   - `new` mallocs, sets refcount=1, runs the ctor (which fills fields in place, returns void)
//   - `const b = a` copies the POINTER -> both alias the same heap object (reference semantics)
//   - RC is deferred: retain/release are centralized stubs; only the refcount-init runs yet (leaks)
class Counter {
    count: int32;
    ctor(start: int32) { this.count = start; }
    get(): int32 { return this.count; }
    plus(n: int32): int32 { return this.count + n; }
}

fn main(): int32 {
    const c = new Counter(40);
    const alias = c;                 // aliases the SAME heap object
    const viaField  = c.count;       // 40  -- field read through the pointer
    const viaMethod = alias.plus(2); // 42  -- method call on the alias
    return viaField + viaMethod - c.get();   // 40 + 42 - 40 = 42
}
