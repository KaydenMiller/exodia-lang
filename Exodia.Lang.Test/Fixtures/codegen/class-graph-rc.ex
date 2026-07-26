// class object-graph RC (§17 increment 2b) -- objects that HOLD references.
//   - store a class ref into a field -> the field RETAINS it (becomes a co-owner)
//   - reassigning a field releases the OLD value first, then retains the new
//   - freeing an object releases its class-typed fields (recursive drop)
//   - class fields are calloc'd to null; releasing an uninitialized (null) field is a no-op
//   - a `new`/call TEMPORARY passed inline as an argument is released after the call
//   validated ASan-clean (no leaks / double-free / use-after-free).
class Box {
    v: int32;
    ctor(x: int32) { this.v = x; }
    get(): int32 { return this.v; }
}

class Holder {
    mut item: Box;
    ctor(seed: Box) { this.item = seed; }        // store-into-field: retains seed
    set(b: Box): bool { this.item = b; return true; }  // reassign: releases old, retains new
    peek(): int32 { const i = this.item; return i.get(); }
}

fn main(): int32 {
    const holder = new Holder(new Box(99));       // temporary Box(99) stored -> retained; temp released
    if (holder.peek() == 99) {
        const box = new Box(42);                  // box owns +1
        holder.set(box);                          // item retains box; old Box(99) released
    }                                             // box scope exit: release (holder.item still owns it)
    return holder.peek();                         // 42 -- holder's field kept it alive across box's scope
    // main exit: release holder -> rc 0 -> free holder, which releases holder.item (Box(42)) -> free
}
