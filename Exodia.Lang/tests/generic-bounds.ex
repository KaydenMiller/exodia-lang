// generic-bounds.ex -- inline single-bound generic constraints `<T: IFoo>`.
//
//   type_parameters : LT type_parameter (',' type_parameter)* GT ;
//   type_parameter  : identifier (COLON type)? ;
//
// ONE bound per parameter (inline). Multiple bounds per param (`<T: A + B>`)
// are the deferred `where`-clause increment. Enforcement ("T must implement the
// bound") is a future semantic check -- the parser only records the bound.

interface Comparable {
    CompareTo(other: int32): int32;
}
interface Display {
    Show(): String;
}

// --- generic FUNCTION with a bound ---
fn max<T: Comparable>(a: T, b: T): T => a;

// --- multiple params: one bounded, one not ---
fn pick<T: Comparable, U>(a: T, b: U): T => a;

// --- bound that is itself a GENERIC type (nested `>>` closes as GT GT) ---
//     (IContainer<int32> is illustrative; generic interfaces aren't declarable yet)
fn label<T: IContainer<int32>>(x: T): int32 => 0;

// --- generic STRUCT with a bound ---
struct SortedList<T: Comparable> {
    private items: T[];
}

// --- generic CLASS: one bounded param, one not ---
class Cache<K: Comparable, V> {
    private keys: K[];
}

// --- generic METHOD with a bound ---
class Mapper {
    public Transform<U: Display>(input: int32): int32 => input;
}

// --- generic IMPL: bound on the impl's own type parameter ---
interface ISized {
    Size(): int32;
}
struct Box<T> {
    private value: T;
}
impl<T: Comparable> ISized for Box<T> {
    Size(): int32 => 1;
}

// --- regression: bound-LESS generics still parse unchanged ---
fn identity<T>(value: T): T => value;
struct Pair<A, B> {
    first:  A;
    second: B;
}
