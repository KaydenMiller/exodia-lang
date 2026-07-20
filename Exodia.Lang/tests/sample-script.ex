/**
 * sample-script.ex -- a fuller example exercising much of the current grammar.
 * The reference notes below match the current design (see DECISIONS.md / README.md).
 *
 * # DECLARATIONS
 * namespace  -> declarative scope for library-level values (decoupled from files)
 * class      -> entity: reference semantics, heap-allocated, identity, `extends`
 * struct     -> value object: immutable, value equality, stack-allocated (LLVM struct)
 * enum       -> sum type with optional payloads (Some(T), Rect(double, double))
 * fn         -> a namespace/global function (methods inside a type omit `fn`)
 *
 * # BINDINGS
 * const      -> immutable binding
 * mut        -> mutable binding
 *
 * # ACCESS
 * global     -> accessible anywhere
 * public     -> accessible to anything that can see the parent
 * internal   -> accessible to siblings of the parent
 * protected  -> accessible to inheritors
 * private    -> accessible only within the declaring type
 *
 * # PRIMITIVE TYPES  (lowercase; each lowers directly to an LLVM scalar)
 * int8..int64    -> signed integers    (i8..i64)
 * uint8..uint64  -> unsigned integers   (i8..i64; signedness enforced by the compiler)
 * float          -> 32-bit IEEE float   (LLVM float)
 * double         -> 64-bit IEEE float   (LLVM double)
 * bool           -> true | false        (i1)
 * char           -> a Unicode scalar value (i32)
 *
 * # LIBRARY TYPES  (PascalCase; representation/ops defined in software)
 * String         -> array of `char` + supporting functions ({ptr, len})
 * Decimal        -> 128-bit base-10 number + software math (literal suffix `m`)
 * Option<T>      -> Some(T) | None            (absence)
 * Result<T, E>   -> Ok(T)  | Err(E)           (recoverable failure)
 * Field<T>       -> Absent | Null | Value(T)  (JSON/DB tri-state)
 *
 * # CONTROL / ERRORS
 * match          -> the only shape-dispatch construct (no `switch`)
 * give           -> produce a block/arm's value
 * panic <expr>   -> unrecoverable escape hatch (unchecked)
 * expr?          -> propagate Err/None to the caller
 * expr!!         -> force-unwrap (panics on Err/None)
 */

namespace StandardLibrary {
    public struct String {
        private chars: char[];

        public static IndexOf(inputStr: String, charToIndex: char) : int64 {
            for (mut i: int32 = 0; i < chars.Length; i += 1) {
                if (chars[i] == charToIndex) {
                    return i;
                }
            }
            return -1;
        }
    }

    public struct Temperature {
        private fahrenheit: double;

        public ctor FromFahrenheit(f: double) {
            this.fahrenheit = f;
        }

        public ctor FromCelsius(c: double) {
            this.fahrenheit = c * (9/5) + 32;
        }

        public GetFahrenheit(): double {
            return fahrenheit;
        }

        public GetCelsius(): double {
            return (this.fahrenheit - 32) / 1.8;
        }
    }

    public struct Date {

    }

    public fn extractValueFromString(input: String) : String {
        const indexOfColon: uint16 = String.IndexOf(input, ':');
        return String.SubString(input, 0, indexOfColon);
    }
}

namespace MyPersonLib {
    public class Person {
        public Name: StandardLibrary::String;
        public Birthday: StandardLibrary::Date;
        public FavoriteTemp: StandardLibrary::Temperature;

        public ctor(
                name: StandardLibrary::String,
                birthday: StandardLibrary::Date,
                favoriteTemp: double) {
            this.Name = name;
            Birthday = birthday;
            FavoriteTemp = new StandardLibrary::Temperature.FromCelsius(favoriteTemp);
        }

        public GetName(): StandardLibrary::String {
            return this.Name;
        }

        public CalculateAge() : int32 {
            return 18;
        }
    }
}

namespace MyCustomProgram {
    public fn PersonFactory(args: StandardLibrary::String[]): MyPersonLib::Person {
        const person = new Person("Kayden", new StandardLibrary::Date("06/18/2025"));
        return person;
    }
}

// Acts as the main starting point for the application
global fn Main(args: StandardLibrary::String[]): int32 {
    const factory = MyCustomProgram::PersonFactory;

    const person = factory(args);
    const personName = person.GetName();
    const personAge = person.CalculateAge();

    StandardLibrary::Console.WriteLine(personName + " is " + personAge);

    return 0;
}
