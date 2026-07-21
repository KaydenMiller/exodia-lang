// where-clause.ex -- C#-style `where` generic constraints.
//
//   where_clause : WHERE identifier COLON type (',' type)* ;
//
// One `where` per type parameter; multiple bounds are COMMA-separated (C# style,
// no `+` combiner -> no ADDITIVE_OPERATOR token clash). Placed after the
// signature, before the body/brace, at every generic site. Composes with inline
// bounds. Enforcement ("T actually implements the bound") is semantic/later.

interface IFoo { A(): int32; }
interface IBar { B(): int32; }
interface IHash { H(): int32; }
interface IComp { C(other: int32): int32; }
class Base { }

// --- fn: single where clause, before an expression body ---
fn one<T>(x: T): T
    where T: IFoo
    => x;

// --- fn: multiple where clauses (one per param) + multiple bounds (comma) ---
fn many<T, TErr>(input: T): T
    where T: IFoo, IBar, IHash
    where TErr: IComp
    => input;

// --- compose: one param bounded INLINE, another via WHERE ---
fn mixed<T: IFoo, U>(a: T, b: U): T
    where U: IBar
    => a;

// --- struct with where ---
struct Sorted<T>
    where T: IComp
{
    private items: T[];
}

// --- class with `extends` AND where (where follows extends) ---
class Registry<K, V> extends Base
    where K: IComp, IHash
    where V: IFoo
{
    private keys: K[];
}

// --- enum with where ---
enum Wrapped<T>
    where T: IFoo
{
    Some(T),
    None,
}

// --- impl with where ---
struct Box<T> { private value: T; }
impl<T> IFoo for Box<T>
    where T: IComp
{
    A(): int32 => 0;
}

// --- interface METHOD SIGNATURE with where ---
interface IMapper {
    Map<U>(input: int32): U where U: IFoo;
}

// --- regression: no where clause still parses ---
fn plain<T>(x: T): T => x;
