// logical-not.ex -- unary ! on a runtime bool.
fn main(): int32 {
    mut x = 5;
    if (!(x > 10)) {    // x=5 -> (x>10)=false -> !false=true -> take
        return 20;
    }
    return 0;
}
