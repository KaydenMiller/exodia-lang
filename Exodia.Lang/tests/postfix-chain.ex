// postfix-chain.ex -- unified postfix chain. `.`, `[]`, call, `?`, `!!` now all
// follow ANY primary (call results, parenthesized/cast exprs, `new`, `?`). This
// replaced the old member_expression/call_expression split, and it's what makes
// the `(value as IFoo).Method()` dispatch form work.
//
//   postfix_expression : primary_expression postfix_op* ;
//   postfix_op : '.' identifier | '[' expression ']' | arguments | QUESTION | DOUBLE_BANG ;
//
// Assignment targets are the same chain (lvalue-ness is a semantic check).

interface IShape { Area(): double; }
struct Circle { public ctor(r: double) { } }
impl IShape for Circle { Area(): double => 0.0; }

fn chains(val: dyn IShape, id: int32, i: int32): int32 {
    // --- member after a CALL (was impossible: member only followed qualified_name) ---
    const a = getConfig().timeout;

    // --- member after `?` (the limitation we flagged when building `?`) ---
    const b = getUser(id)?.name;

    // --- member + call after a PARENTHESIZED CAST: the qualified-dispatch form ---
    const c = (val as IShape).Area();

    // --- member + call after `new` ---
    const d = new Circle(1.0).Area();

    // --- index after a call; call after an index ---
    const e = getList()[0];
    handlers[i]();

    // --- chained calls (call the result of a call) ---
    const f = curry()();

    // --- a deep mixed chain: member, index, call, ?, call, !! ---
    const g = obj.parts[0].build()?.finish()!!;

    return 0;
}

// --- assignment to a chained PLACE (the LHS is the chain) ---
fn places(): int32 {
    obj.inner.value = 5;
    arr[i].count = arr[i].count + 1;
    return 0;
}

// --- UFCS static call still parses (qualified_name + call) ---
fn ufcs(x: int32): double => IShape::Area(x);

// --- qualified member then call (namespace path + method) ---
fn qualified(): int32 {
    StandardLibrary::Console.WriteLine("hi");
    return 0;
}
