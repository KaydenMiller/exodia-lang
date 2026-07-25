// default-params.ex -- default parameter values on fn / method / ctor params.
//
//   formal_parameter : identifier COLON type ('=' assignment_expression)? ;
//
// The default is any assignment_expression. NOTE: `= default` (the target-typed
// default EXPRESSION) is DEFERRED and intentionally not used here -- these are
// concrete default values only.

// --- literal defaults of several kinds (int / string / bool / char / double) ---
fn config(
    retries: int32  = 3,
    label:   String = "default",
    enabled: bool   = true,
    sep:     char   = ',',
    rate:    double = 1.5): int32 {
    return retries;
}

// --- defaults that are expressions / enum values / calls / array literals ---
fn build(
    count:  int32         = 1 + 2,
    maybe:  Option<int32> = None,
    field:  Field<String> = Absent,
    seeded: Field<String> = Value("x"),
    items:  int32[]       = []): int32 {
    return count;
}

// --- required param (no default) followed by a defaulted one ---
fn greet(name: String, greeting: String = "Hello"): String {
    return greeting;
}

// --- default on a CONSTRUCTOR parameter ---
struct Circle {
    private radius: double;

    public ctor(radius: double = 1.0) {
        this.radius = radius;
    }
}

// --- default on a METHOD parameter ---
class Logger {
    public Log(message: String, level: int32 = 0): int32 {
        return level;
    }
}
