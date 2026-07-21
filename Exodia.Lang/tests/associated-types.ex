// associated-types.ex -- generic interfaces, access modifiers, and the `->`
// associated-type (output) clause.
//
//   interface_declaration : accessability_modifier* INTERFACE identifier
//       type_parameters? interface_outputs? interface_extends? where_clause*
//       '{' interface_member* '}' ;
//   interface_outputs : ARROW type_parameter (',' type_parameter)* ;
//   impl_declaration  : IMPL type_parameters? type impl_outputs? FOR type
//       where_clause* '{' method_declaration* '}' ;
//   impl_outputs      : ARROW type (',' type)* ;
//
// `<inputs>` for inputs (caller-chosen), `-> outputs` for associated types
// (impl-determined). Outputs live in the HEADER, not the body. Enforcement
// (a type actually implements the interface / binds its outputs) is semantic.

interface IComparable { CompareTo(other: int32): int32; }
interface IBase { Base(): int32; }

// --- access modifier on an interface ---
public interface IShape {
    Area(): double;
}

// --- generic interface (inputs in <>) ---
interface Container<T> {
    Add(item: T): int32;
}

// --- ONE associated type (output): the operator pattern ---
interface Mul<Rhs> -> Output {
    Multiply(rhs: Rhs): Output;
}

// --- PURE output: no <> at all (the Iterator pattern) ---
interface Iterator -> Item {
    Next(): Option<Item>;
}

// --- MULTIPLE outputs (comma-list) ---
interface Graph -> Node, Edge {
    AddNode(): Node;
    AddEdge(): Edge;
}

// --- BOUNDED associated type ---
interface Sorted -> Item: IComparable {
    First(): Item;
}

// --- generic interface with a where clause ---
interface Keyed<K> where K: IComparable {
    KeyOf(): K;
}

// --- everything: modifier + generics + output + extends + where ---
public interface Full<T> -> Out extends IBase where T: IComparable {
    Process(input: T): Out;
}

// --- IMPL binding a single output (the operator impl) ---
struct Vector2 { public x: double; public y: double; }
impl Mul<Vector2> -> double for Vector2 {
    Multiply(rhs: Vector2): double => this.x * rhs.x + this.y * rhs.y;
}

// --- IMPL binding MULTIPLE outputs ---
struct IntGraph { }
impl Graph -> int32, int32 for IntGraph {
    AddNode(): int32 => 0;
    AddEdge(): int32 => 0;
}

// --- generic impl of a PURE-output interface, with where ---
struct List<T> { private items: T[]; }
impl<T> Iterator -> T for List<T>
    where T: IComparable
{
    Next(): Option<T> => None;
}

// --- regression: a plain interface (no modifier / generics / outputs) ---
interface Plain {
    Foo(): int32;
}
