// generics.ex
// type_parameters  = DECLARING <T> on struct / class / function / method
// type_arguments   = USING <String> wherever a type appears (fields, params, returns)

// --- generic struct: type parameter used as a field type ---
struct Box<T> {
    value: T;
}

// --- multiple type parameters ---
struct Pair<A, B> {
    first:  A;
    second: B;
}

// --- generic class ---
class Container<T> {
    private items: T[];
}

// --- type ARGUMENTS in field types (fields USE generics; they don't declare them) ---
struct Registry {
    opt:    Option<String>;
    result: Result<int32, ParseError>;
    nested: Map<String, List<int32>>;    // closes as GT GT in TYPE position
    optArr: Option<String[]>;            // array inside a generic
    genArr: List<int32>[];               // array of a generic
}

// --- generic method on a generic type ---
class Mapper<T> {
    public Transform<U>(input: T): U {
        return input;
    }
}

// --- generic free functions (needs type_parameters on function_declaration) ---
fn identity<T>(value: T): T {
    return value;
}

fn combine<T, U>(a: T, b: U): int32 {
    return 0;
}

// --- regression: >> / << in EXPRESSION position still parse (GT GT is context-sensitive) ---
fn shiftCheck(): int32 {
    mut x = 8;
    x = x >> 1;
    x = x << 2;
    return x;
}
