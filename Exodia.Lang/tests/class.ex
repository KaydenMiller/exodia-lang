// class.ex -- reference types / entities (fields + extends for now;
// modifiers, methods, and constructors arrive via the shared `member` rule later).

// --- qualified (::) base class + fields ---
class Person extends StandardLibrary::Entity {
    Name: String;
    Birthday: StandardLibrary::Date;
}

// --- empty class: member* matches ZERO members ---
class Empty {
}

// --- no `extends`: class_extends? is optional ---
class Standalone {
    id: int64;
}

// --- single-segment base (a qualified_name with one segment) ---
class Admin extends User {
    level: int32;
}

// --- varied field types: array, multi-dimensional, qualified + array ---
class Registry {
    entries: Person[];
    matrix: int32[][];
    names: StandardLibrary::String[];
}
