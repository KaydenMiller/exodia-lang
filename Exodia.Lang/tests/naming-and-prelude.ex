/**
 * naming-and-prelude.ex
 *
 * Demonstrates the two-tier type naming and the prelude.
 *
 *   lowercase  = PRIMITIVE scalars. Lower DIRECTLY to an LLVM scalar type.
 *                Fixed set:
 *                int8..int64, uint8..uint64, float, double, bool, char
 *
 *   PascalCase = LIBRARY types: their representation and/or operations are
 *                defined in software, even if the type "feels" primitive.
 *                Includes every user type.
 *                String, Decimal, Option, Result, Field, and your own types.
 *
 * The casing is a SIGNAL, not an inconsistency, and the boundary is concrete:
 * lowercase = lowers directly to an LLVM scalar; PascalCase = library-backed.
 * `String` and `Decimal` both feel primitive but are library types (a struct
 * + software routines), so both are PascalCase -- same reason for both.
 *
 * Core library types live in the stdlib but are auto-imported by the PRELUDE,
 * so you write `String` / `Option` / `Result` UNQUALIFIED -- never
 * `StandardLibrary::String`. An explicit `::` path is only needed for library
 * items that are NOT in the prelude (see `writeCsv` below).
 *
 * (Ahead of the current grammar, like the other test scripts.)
 *
 * OPEN: how prelude membership is marked -- a `prelude` modifier, a dedicated
 * `Prelude` namespace, or a manifest -- is not yet decided. Shown here via
 * comment only.
 */

namespace StandardLibrary {

    // Library type. PascalCase, built on the `char` primitive.
    // In the prelude -> users write `String`, not `StandardLibrary::String`.
    public struct String {
        private chars: char[];
    }

    // Also "feels primitive" but is library-backed: 128-bit base-10 plus
    // software math (there is no native LLVM decimal). PascalCase, same as String.
    public struct Decimal { }

    // Prelude enums -- also written unqualified everywhere.
    public enum Option<T> { Some(T), None }
    public enum Result<T, E> { Ok(T), Err(E) }
    public enum Field<T> { Absent, Null, Value(T) }

    // NOT in the prelude -- callers must reach it via its `::` path.
    public struct CsvWriter { }
}

namespace App {

    // A user type is PascalCase -- same tier as String / Option.
    // Notice each field's tier is visible at a glance from its casing.
    public struct Measurement {
        // lowercase PRIMITIVE scalars (each lowers directly to an LLVM scalar):
        public celsius:  double;    // 64-bit IEEE float    -> LLVM `double`
        public samples:  uint32;    // unsigned 32-bit int  -> LLVM `i32`
        public verified: bool;      // -> LLVM `i1`
        public grade:    char;      // -> LLVM `i32` (Unicode scalar)

        // PascalCase LIBRARY types, written unqualified thanks to the prelude:
        public label:    String;          // not StandardLibrary::String
        public price:    Decimal;         // library-backed, PascalCase (same reason as String)
        public note:     Option<String>;  // generic library type of a library type
    }

    // A signature mixing both tiers. `String` and `Result` resolve via the prelude.
    public fn summarize(m: Measurement): Result<String, String> {
        if (m.samples == 0) {
            return Err("no samples recorded");
        }
        return Ok(m.label);
    }

    // Contrast: a library type that is NOT in the prelude still needs its path.
    public fn writeCsv(): void {
        const writer = new StandardLibrary::CsvWriter();
    }
}
