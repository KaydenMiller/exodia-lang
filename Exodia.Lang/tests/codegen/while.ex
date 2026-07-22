// while.ex -- loop with a carried accumulator. Sums 1..5 == 15.
// Exercises: while.cond/while.body/while.exit blocks, the back-edge, and mut
// locals carried across iterations via alloca/load/store (no phi nodes).
fn main(): int32 {
    mut i = 1;
    mut sum = 0;
    while (i <= 5) {
        sum = sum + i;
        i = i + 1;
    }
    return sum;
}
