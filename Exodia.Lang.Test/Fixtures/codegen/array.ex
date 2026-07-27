// Array<T> (§23) -- an RC-managed, immutable reference type: { i64 rc, i64 len, [len x T] } inline.
//   layout mirrors String: header at offset 0, elements packed inline at offset 16.
//   - `[a, b, c]` builds an Array (calloc one block, rc=1, store len + each element)
//   - `arr[i]` reads element i (gep to data at obj+16, then gep by index)
//   - `.length` is the element count (i64)
//   - element type is monomorphized per T: Array$i32, Array$String, Array$Box (one named struct each)
//   - RC-managed like any class: retained/released, freed at rc 0.
//   - arrays OF references (String[], class[]) own their elements: each element is retained on
//     store and released when the array is freed (a runtime loop over len). ASan-clean.
extern fn puts(s: cstr): int32;

class Box {
    v: int32;
    ctor(x: int32) { this.v = x; }
    get(): int32 { return this.v; }
}

fn main(): int32 {
    const nums = [10, 20, 30, 40, 50];   // Array$i32
    const n = nums.length;               // 5 (i64)
    const first = nums[0];               // 10
    const last = nums[4];                // 50

    const names = ["alice", "bob"];      // Array$String -- elements retained/released
    puts(names[0]);
    puts(names[1]);

    const boxes = [new Box(3), new Box(4), new Box(5)];   // Array$Box
    const b = boxes[2];                  // borrow -> retained; array still owns the element
    return first + last + b.get();       // 10 + 50 + 5 = 65
    // scope exit: boxes freed -> releases its 3 Box elements; names freed -> releases its 2 Strings
}
