// struct.ex -- value-object shells (fields only for now;
// modifiers, methods, and constructors come in later sub-steps).

// --- basic: primitive-typed fields ---
struct Point {
    x: int32;
    y: int32;
}

// --- qualified (::) field types ---
struct ExtendedPoint {
    x: StandardLibrary::Vector2;
    y: StandardLibrary::Vector2;
}

// --- empty struct: exercises field_declaration* matching ZERO fields ---
struct Empty {
}

// --- single field ---
struct Wrapper {
    value: int64;
}

// --- array-typed field: exercises the [] suffix on the type rule ---
struct Line {
    points: Point[];
}

// --- multi-dimensional array: exercises the repeatable ('[' ']')* ---
struct Grid {
    cells: int32[][];
}

// --- qualified type + array combined ---
struct Names {
    values: StandardLibrary::String[];
}

// --- value-object shape (DDD): unqualified prelude library type + primitive ---
struct Money {
    amount: int64;
    currency: String;
}
