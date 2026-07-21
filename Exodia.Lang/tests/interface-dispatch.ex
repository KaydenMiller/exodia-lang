// interface-dispatch.ex -- static vs dynamic dispatch over ONE interface.
//
// Same DEFINITION and IMPLEMENTATION for both; only the USE differs:
//   - static  : `<T: HasArea>` -- one concrete type, direct calls, type preserved
//   - dynamic : `dyn HasArea`  -- mixed types, vtable calls, type erased
//
// Both halves are real now that `dyn` is built.
// NOTE: generic call args are inferred (`largestByArea(circles)`); explicit
// `<Circle>(...)` at a call site is expression-position generics, deferred (§12).

// ---- DEFINITION ----
public interface HasArea {
    Area(): double;
}

// ---- IMPLEMENTATION (two concrete types, separate impl blocks) ----
struct Circle {
    private radius: double;
    public ctor(radius: double) { this.radius = radius; }
}
impl HasArea for Circle {
    Area(): double => this.radius * this.radius * 3.14159;
}

struct Rectangle {
    private w: double;
    private h: double;
    public ctor(w: double, h: double) { this.w = w; this.h = h; }
}
impl HasArea for Rectangle {
    Area(): double => this.w * this.h;
}

// ---- STATIC: generic over ONE concrete type; `.Area()` is a direct call ----
fn largestByArea<T: HasArea>(items: T[]): T {
    mut best = items[0];
    for (mut i: int32 = 1; i < items.Length; i += 1) {
        if (items[i].Area() > best.Area()) {     // direct call to T's Area, no vtable
            best = items[i];
        }
    }
    return best;                                 // returns a real T (type preserved)
}

global fn Main(args: String[]): int32 {
    const circles: Circle[] = [new Circle(1.0), new Circle(3.0), new Circle(2.0)];
    const biggest = largestByArea(circles);      // T inferred = Circle; result is a Circle
    return 0;
}

// ---- DYNAMIC: `dyn HasArea` -- any implementer, decided at runtime. The ONLY
//      way to hold a MIXED collection (Circle AND Rectangle in one array), which
//      the static `<T>` form cannot express. `.Area()` is a vtable call.
fn totalArea(shapes: dyn HasArea[]): double {
    mut total: double = 0.0;
    for (mut i: int32 = 0; i < shapes.Length; i += 1) {
        total += shapes[i].Area();               // vtable call -- Circle's OR Rectangle's
    }
    return total;
}

global fn Main2(args: String[]): int32 {
    const shapes: dyn HasArea[] = [               // MIXED types in one array
        new Circle(1.0),
        new Rectangle(2.0, 3.0),
        new Circle(0.5),
    ];
    const t = totalArea(shapes);                 // each element dispatches to its own Area
    return 0;
}

// --- dyn also works as a generic arg and a field type ---
struct ShapeBox {
    private shape: dyn HasArea;
}
fn firstOf(shapes: List<dyn HasArea>): dyn HasArea => shapes[0];
