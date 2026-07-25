fn main(args: StandardLibrary::String[]): int32 {
    mut a = 1 + 2 * 3;      // additive/multiplicative (already worked)
    mut b: float = 3.5 - 1.25;     // FLOAT literals            -> proves fix #2 (FLOAT wired into numeric_literal)
    const c: float = 2.7 - 1.0;
    a = a + 1;              // assignment as a statement -> proves fix #1 (expression reaches assignment)
    if (a == 7) {           // == comparison             -> proves fix #1 (expression reaches equality)
        b = b * 2.0;
    }
    print(a);               // call expression           -> proves fix #1 (expression reaches call)
    print(c);
    return a;
}