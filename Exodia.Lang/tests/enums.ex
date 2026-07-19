// enums.ex -- sum types: variants with optional tuple payloads, generics,
// and an optional trailing comma. Empty enums are allowed (a semantic warning later).

// --- simple enum, NO trailing comma ---
public enum Status {
    Active,
    Inactive,
    Disabled
}

// --- generic enum (single param), WITH trailing comma ---
public enum Field<T> {
    Value(T),
    Null,
    Absent,
}

// --- generic enum, no trailing comma ---
public enum Option<T> {
    Some(T),
    None
}

// --- generic enum with TWO type parameters ---
public enum Result<T, E> {
    Ok(T),
    Err(E),
}

// --- multi-type payloads (tuple variants) + a payload-less variant ---
enum Shape {
    Empty,
    Circle(double),
    Rect(double, double),
}

// --- no access modifier ---
enum Color {
    Red,
    Green,
    Blue,
}

// --- empty enum: allowed by the grammar (warned about later, not rejected) ---
enum Marker {
}

// --- enum INSIDE a namespace (needs enum_declaration in namespace_member) ---
namespace Lib {
    public enum ParseError {
        Empty,
        NotANumber,
        OutOfRange,
    }
}
