// array-literal.ex -- exercises the array_literal expression.
//
//   array_literal : '[' (expression (',' expression)* ','?)? ']' ;
//   hooked into primary_expression, so it's usable anywhere an expression is.
//
// Reminder: element-type inference (empty [] needs a type from context) and
// element-type consistency are SEMANTIC checks, not grammar -- the parser
// accepts all of these shapes.

fn shapes(): int32 {
    // --- empty literal (type comes from the annotation) ---
    mut empty: int32[] = [];

    // --- single element ---
    const one: int32[] = [1];

    // --- multiple elements ---
    const many: int32[] = [1, 2, 3, 4];

    // --- trailing comma (matches enum / match-arm style) ---
    const trailing: int32[] = [1, 2, 3,];

    // --- elements are full expressions, not just literals ---
    const exprs: int32[] = [1 + 2, one[0], many[1] * 3];

    // --- nested arrays (element is itself an array_literal) ---
    const grid: int32[][] = [[1, 2], [3, 4], [5, 6]];

    // --- non-numeric element types ---
    const words: String[] = ["a", "b", "c"];
    const chars: Char[]   = ['x', ',', '\n'];

    // --- array literal as a call argument ---
    Sum([10, 20, 30]);

    // --- array literal as the scrutinee of a match ---
    const kind = match classify([1, 2]) {
        0 => "empty",
        _ => "nonempty",
    };

    return 0;
}
