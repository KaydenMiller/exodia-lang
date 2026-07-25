// named-arguments.ex -- named arguments in calls and `new`.
//
//   argument : (identifier ':')? assignment_expression ;
//
// Name syntax is `name: value` (matches C#). Ordering (positional-before-named)
// is a SEMANTIC rule, not grammar -- mixed orders parse here; the type-checker
// enforces the convention and binds names to the callee's parameters later.

fn callSites(): int32 {
    doThing(1, 2, 3);                                  // all positional (regression)
    doThing(a: 1, b: 2, c: 3);                         // all named
    doThing(1, b: 2, c: 3);                            // mixed: positional then named

    // named values are full expressions: arithmetic, call, array
    build(count: 1 + 2, field: Value("x"), items: [1, 2, 3]);

    // nested: named args inside a named arg's value
    outer(inner: compute(x: 10, y: 20));

    return 0;
}

// --- named args on a CONSTRUCTOR via `new` ---
fn makePerson(): Person {
    return new Person(name: "Kayden", age: 30, nickname: Null);
}

// --- disambiguation: identifier-led POSITIONAL expressions must NOT be read as named ---
fn disambiguation(a: int32, b: int32): int32 {
    takesExpr(a);          // positional bare identifier   -> NOT named
    takesExpr(a + b);      // positional expr starting with an identifier
    takesExpr(a.field);    // positional member access
    takesTwo(x: a, y: b);  // named, whose values are bare identifiers
    return 0;
}
