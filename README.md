# EXODIA

A statically-typed, AOT-compiled language (LLVM target), C#/Rust-flavored:
explicit over implicit, "no magic," OOP + functional + imperative.

Current stage: **grammar (ANTLR4 → C#) + parser**. The 22 files in
`Exodia.Lang/tests/` all parse. Semantic analysis, type checking, and codegen
are not built yet — so anything below described as a "semantic check" (range,
exhaustiveness, mutability, conversions, "must be an exception type", etc.)
is a *future* compiler pass, not something the parser enforces today.

The authoritative design record is `DECISIONS.md`. This README is a summary of
what is currently decided and parsing; where the two ever differ, `DECISIONS.md`
wins.

## Useful tools
- https://astexplorer.net/
- https://godbolt.org/
- https://sharplab.io/

# Design Goals
- First-class functions (assign, pass, return)
- Elegant yet explicit — no magic
- Namespacing (declarative, decoupled from files)
- OOP + functional + imperative
- Statically typed, compiled ahead-of-time to LLVM
- No `null`; absence and failure are modeled in the type system

# Type System

## Two naming tiers
The casing of a type tells you how it is represented:
- **lowercase = primitive** — lowers directly to an LLVM scalar:
  `int8..int64`, `uint8..uint64`, `float`, `double`, `bool`, `char`.
  (`char` is a Unicode scalar → LLVM `i32`. Signedness is enforced by the
  compiler, not LLVM: `int32`/`uint32` both lower to `i32`.)
- **PascalCase = library-backed** — representation and/or operations defined in
  software, even if it "feels" primitive: `String`, `Decimal`, `Option`,
  `Result`, `Field`, and all user types.

One canonical name per type — no C#-style `int`/`Int32` dual naming.

```exodia
public struct Measurement {
    public celsius:  double;          // -> LLVM double
    public samples:  uint32;          // -> LLVM i32
    public verified: bool;            // -> LLVM i1
    public grade:    char;            // -> LLVM i32 (Unicode scalar)

    public label:    String;          // library type (PascalCase)
    public price:    Decimal;         // library-backed, PascalCase
    public note:     Option<String>;  // generic library type
}
```

## Numeric literals
- Integer and float literals; `_` digit separators (`1_000`, `1_000.00`).
- Optional short type suffixes: `i8..i64`, `u8..u64`, `f` (float), `d` (double),
  `m` (`Decimal`, exact base-10).
- Go-style "untyped constant" typing: a literal takes the annotated/contextual
  type, otherwise an eager default (`int32` for integers, `double` for reals).

## Prelude
Core library types (`String`, `Decimal`, `Option`, `Result`, `Field`) are
auto-imported and written unqualified. Prelude names are *weak* — an explicit
declaration/import of the same name wins.

# Bindings & Mutability
- `const` = immutable binding, `mut` = mutable binding (there is no `let`).
- Types are immutable by default; a `mut` field opts into mutability.
- `const` binding + immutable type = deeply locked; `const` binding + type with
  `mut` fields = shallow. No borrow checker required.

```exodia
const x: int32 = 1 + 2 * 3;
mut   y: float = 3.5 - 1.25;
y = y * 2.0;
```

# Functions, Methods & Construction
- `fn` for namespace/global functions; **methods inside a type omit `fn`**.
- Parameters are always typed; the **return type is always declared** — use
  `void` (a real unit type) when nothing is returned.
- Two body forms: a **block** `{ … }` (always uses an explicit `return`), or an
  **expression body** `=> expr;` (the expression is the implicit return).
- **Default parameter values**: `age: int32 = 0`.
- **Named arguments** at call sites: `f(name: "x", age: 30)` — works for `new` too.

```exodia
fn add(a: int32, b: int32): int32 => a + b;      // expression body

global fn Main(args: String[]): int32 {          // block body
    const sum = add(a: 2, b: 3);                  // named arguments
    return 0;
}
```

## Constructors
`ctor` keyword (not the class name). Named and unnamed forms; invoked with `new`.
Validated construction uses a private `ctor` + a static `Create(): Result<T, E>`
(smart constructor).

```exodia
public struct Temperature {
    private fahrenheit: double;

    public ctor FromCelsius(c: double = 0.0) {    // named ctor + default value
        this.fahrenheit = c * (9 / 5) + 32;
    }
    public GetFahrenheit(): double => this.fahrenheit;   // expression-bodied method
}

const t = new Temperature.FromCelsius(c: 100.0);
```

# struct vs class
- **`struct` = value object** (DDD): immutable, value equality, value semantics,
  stack-allocated → LLVM struct, no heap/GC. No inheritance. "To change, make a new one."
- **`class` = entity**: reference semantics, identity, heap-allocated; `extends`
  a single base (qualified name).

# Enums (sum types)
Variants with optional tuple payloads, generics, optional trailing comma.

```exodia
public enum Option<T> { Some(T), None }
public enum Result<T, E> { Ok(T), Err(E) }
public enum Field<T> { Absent, Null, Value(T) }
public enum Shape { Empty, Circle(double), Rect(double, double) }
```

# Generics
Restricted to **type positions** (sidesteps `<`-vs-comparison ambiguity).
`type_parameters` on struct/class/function/method; `type_arguments` wherever a
type appears. Nested generics close fine (`Map<String, List<int32>>`).

```exodia
struct Box<T> { value: T; }
fn identity<T>(value: T): T => value;
```

# Control Flow
- `if` / `else` / `else if` (the last falls out of `else` + nested `if`).
- `while`, `do { … } while (cond);` (do-while **requires** a trailing `;`),
  and C-style `for (mut i: int32 = 0; i < n; i += 1) { … }`.
- Bodies may be braced **or** a single brace-less statement (`if (x) return 0;`).
- There is no `++`/`--` — use `i += 1`.

# Pattern Matching
`match` is the only branching-on-shape construct (no `switch`). It is both an
**expression** (produces a value) and a **statement** (side effects, value
discarded), and — like `if`/loops — needs no trailing `;` as a statement.

Arms are `pattern (when guard)? => body`. Patterns: variant destructure
(`Ok(v)`, `Some(x)`, `None`), literal (`200`, `'a'`), OR (`200 | 201`), range
(`200..299`, `'a'..'z'`), and wildcard `_`. Bodies are an expression, or a block
that produces its value with **`give`**.

```exodia
fn classify(code: int32): String =>
    match code {
        200 | 201 | 204 => "success",
        400..499        => "client error",
        500             => {
            Metrics::Increment("errors");
            give "server error";      // block arm produces its value with `give`
        },
        _               => "unknown",
    };
```

# Errors & Absence — no `null`
Absence and failure live in the type system, handled three ways:

| Concern | Tool |
|---|---|
| A value may be absent | `Option<T>` (`Some`/`None`) |
| A call may fail (recoverable) | `Result<T, E>` (`Ok`/`Err`) |
| JSON/DB tri-state (absent vs explicit null vs value) | `Field<T>` (`Absent`/`Null`/`Value`) — always `match`ed |
| Unrecoverable ("should never happen") | `panic <expr>` (unchecked; not in signatures) |

`Option`/`Result`/`Field` are ordinary library enums — no compiler magic.

**Operators** (postfix): `?` = **propagate** (`Ok`/`Some` → unwrap; `Err`/`None`
→ early-return from the function). `!!` = **force-unwrap** (panics on `Err`/`None`).

```exodia
fn parseSum(a: String, b: String): Result<uint16, ParseError> {
    const first  = parseUint16(a)?;   // on Err, parseSum returns Err here
    const second = parseUint16(b)?;
    return Ok(first + second);
}
```

**EXIT vs PRODUCE** — the guiding rule:
- `return` / `?` / `panic` **exit the function**.
- `give` **produces** a block/match-arm's value (execution continues).

# Namespaces
C#-style **declarative namespaces, decoupled from files**: multiple per file,
nestable, `::`-qualified. (This is explicitly *not* Rust/TS file-path modules,
so a project/manifest will be needed to enumerate source files.)

```exodia
namespace Company::Product {
    public struct Widget { private id: int64; }
}

namespace Outer {
    namespace Inner {
        public fn ping(): int32 => 1;
    }
}
```

# Toolchain
ANTLR4 grammar (`Exodia.Lang/Exodia.g4`), C# target. The parser is regenerated
**manually** via a Docker `antlr4` alias (no build-time codegen) — regenerate
after every `.g4` edit or a run uses a stale parser:

```
# from inside Exodia.Lang/
antlr4 -o Antlr Exodia.g4
```

Tests are `.ex` files under `Exodia.Lang/tests/`, run by piping into the parser
(`Program.cs` reads source on stdin and prints the parse tree).

# Not yet built (roadmap)
Deferred in `DECISIONS.md`: lambdas (`=>` arrow functions) · interfaces/traits
(needed for DI inversion, generic constraints, the `Try` interface behind `?`,
and `Default`) · `try`/`catch`/`finally` (panic recovery) and a Rust-style
`try { }` propagation-boundary expression · `using`/`await using` + async ·
`as` cast · `with` expression + compiler-generated value equality · attributes/
metadata · primary-constructor shorthand · cascade `..` · bitwise operators ·
compile-time DI tooling · project/manifest + batch resolver.
