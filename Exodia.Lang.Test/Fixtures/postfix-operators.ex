// postfix-operators.ex -- the `?` (propagate) and `!!` (force-unwrap) postfix ops.
//
//   unary_expression   : postfix_expression | ADDITIVE_OPERATOR unary_expression ;
//   postfix_expression : (primary_expression | call_expression) postfix_op* ;
//   postfix_op         : QUESTION | DOUBLE_BANG ;
//
//   ?   propagate Err/None as an early return (safe)
//   !!  force-unwrap, panicking on Err/None (unsafe)
//
// These are SYNTAX only; which types they're valid on is a type-checker rule
// (waits on interfaces). Semantically-nonsense forms like `5?` still parse.
//
// KNOWN LIMITATION (documented, intentionally NOT tested here): member access
// AFTER a postfix op -- `getUser(id)?.name` -- does not parse yet, because
// `.`/`[]` live in member_expression, which doesn't wrap postfix_expression.

// --- `?` after a call, in a const initializer ---
fn propagate(x: String): Result<uint16, ParseError> {
    const first = parse(x)?;
    return Ok(first);
}

// --- `!!` after a call, in a const initializer ---
fn force(x: String): uint16 {
    const value = parse(x)!!;
    return value;
}

// --- both as expression-bodied function bodies ---
fn propBody(x: String): uint16 => parse(x)?;
fn forceBody(x: String): uint16 => parse(x)!!;

// --- postfix on a bare identifier (primary) and on an indexed element (member) ---
fn onPrimaries(opt: Option<uint16>, arr: Option<uint16>[]): uint16 {
    const a = opt?;        // identifier ?
    const b = arr[0]?;     // arr[0] is a member_expression (index) -> primary
    const c = opt!!;       // identifier !!
    return a + b + c;
}

// --- binds tighter than arithmetic: `parse(a)? + parse(b)?` = `(parse(a)?) + (parse(b)?)` ---
fn inArithmetic(a: String, b: String): Result<uint16, ParseError> {
    return Ok(parse(a)? + parse(b)?);
}

// --- `?` inside a call argument ---
fn asArgument(x: String): Result<uint16, ParseError> {
    return Ok(double(parse(x)?));
}

// --- stacked postfix ops (the postfix_op* loop allows it; semantics decide sense) ---
fn stacked(x: String): uint16 => parse(x)?!!;
