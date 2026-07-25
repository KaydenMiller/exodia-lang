// namespace.ex -- the outer container: namespaces holding declarations
// (struct / class / fn / nested namespace), plus top-level global functions.

// --- basic: a namespace with a struct and a function ---
namespace StandardLibrary {
    public struct Date {
    }

    public fn extractValue(input: String): String {
        return input;
    }
}

// --- empty namespace: namespace_member* matches ZERO members ---
namespace Empty {
}

// --- qualified namespace name (Company::Product) ---
namespace Company::Product {
    public struct Widget {
        private id: int64;
    }
}

// --- nested namespaces (namespace_member includes namespace_declaration) ---
namespace Outer {
    namespace Inner {
        public fn ping(): int32 {
            return 1;
        }
    }
}

// --- a fuller namespace: class + struct + fn together, with modifiers ---
namespace MyPersonLib {
    public class Person extends StandardLibrary::Entity {
        private Name: String;

        public ctor(name: String) {
            this.Name = name;
        }

        public GetName(): String {
            return this.Name;
        }
    }

    public struct Money {
        private amount: int64;
    }

    public fn helper(x: int32): int32 {
        return x;
    }
}

// --- top-level global function, OUTSIDE any namespace ---
global fn Main(args: String[]): int32 {
    return 0;
}
