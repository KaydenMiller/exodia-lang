// struct.ex -- value objects: fields, modifiers, methods, and constructors (ctor).

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
    private points: Point[];
}

// --- multi-dimensional array: exercises the repeatable ('[' ']')* ---
private struct Grid {
    private cells: int32[][];
}

// --- qualified type + array combined ---
protected struct Names {
    private values: StandardLibrary::String[];
}

// --- value-object shape (DDD): fields + unnamed ctor + behavior ---
public struct Money {
    private amount: int64;
    private currency: String;

    // unnamed constructor
    public ctor(amount: int64, currency: String) {
        this.amount = amount;
        this.currency = currency;
    }

    // behavior returns a NEW value (value objects never mutate)
    public Add(other: Money): Money {
        return new Money(this.amount + other.amount, this.currency);
    }

    // read accessor: publicly-immutable / privately-mutable (option 3)
    public Amount(): int64 {
        return this.amount;
    }
}

// --- Value object to represent domain invariants with static factory method
public struct Money {
    private amount: int64;
    private currency: String;

    // Raw constructor is PRIVATE — can't be called from outside, so it can't be
    // used to bypass validation. It just assigns; it enforces nothing.
    private ctor(amount: int64, currency: String) {
        this.amount = amount;
        this.currency = currency;
    }

    // The ONLY public construction path — validates, then builds on success.
    public static Create(amount: int64, currency: String): Result<Money, ValidationError> {
        if (amount < 0) {
            return Err(NegativeAmount);
        }
        return Ok(new Money(amount, currency));   // private ctor reachable from inside the type
    }
}

// --- named constructors: the identical-(double)-signature case + a void method ---
public struct Temperature {
    private fahrenheit: double;

    public ctor FromFahrenheit(f: double) {
        this.fahrenheit = f;
    }

    public ctor FromCelsius(c: double) {
        this.fahrenheit = c * (9.0 / 5.0) + 32.0;
    }

    public GetFahrenheit(): double {
        return this.fahrenheit;
    }

    public Print(): void {
        return;
    }
}
