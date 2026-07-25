// ast-arith.ex -- binary arithmetic through the AST path.
fn main(): int32 {
    return 1 + 2 * 3 - 10 / 2;   // 1 + 6 - 5 = 2  (precedence: * and / bind tighter)
}
