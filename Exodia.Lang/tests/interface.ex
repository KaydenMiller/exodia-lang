// interface.ex -- interface declarations + impl blocks (first increment).
//
//   interface_declaration : INTERFACE identifier '{' method_signature* '}' ;
//   method_signature      : identifier type_parameters? '(' formal_parameter_list? ')' COLON type SEMI ;
//   impl_declaration      : IMPL type_parameters? type FOR type '{' method_declaration* '}' ;
//
// Interface methods are SIGNATURES only (no bodies -- default methods deferred).
// impl methods are full method_declarations (bodies), implicit `this` receiver.
//
// NOT in this increment (deferred): access modifiers on interfaces (`public
// interface`), generic interfaces (`interface Foo<T>`), `<T: IFoo>` bounds,
// associated types, default method bodies, `dyn`, and the `(x as IFoo)` forms.

// --- a simple interface: method signatures only ---
interface IShape {
    Area(): double;
    Perimeter(): double;
    Describe(name: String): String;
}

// --- a value object implementing it (impl is a SEPARATE block) ---
struct Circle {
    private radius: double;
    public ctor(radius: double) { this.radius = radius; }
}
impl IShape for Circle {
    Area(): double      => this.radius * this.radius * 3.14159;
    Perimeter(): double => 2.0 * 3.14159 * this.radius;
    Describe(name: String): String => name;
}

// --- a SECOND type implementing the SAME interface (many impls, no conflict) ---
struct Rectangle {
    private w: double;
    private h: double;
    public ctor(w: double, h: double) { this.w = w; this.h = h; }
}
impl IShape for Rectangle {
    Area(): double      => this.w * this.h;
    Perimeter(): double => 2.0 * (this.w + this.h);
    Describe(name: String): String => name;
}

// --- DI-style: one interface, many implementations (the IBlobStorage sketch) ---
interface IBlobStorage {
    Upload(key: String, data: uint8[]): Result<int32, StorageError>;
    Download(key: String): Result<uint8[], StorageError>;
    Delete(key: String): Result<int32, StorageError>;
}

class AwsBlobStorage {
    private region: String;
    public ctor(region: String) { this.region = region; }
}
impl IBlobStorage for AwsBlobStorage {
    Upload(key: String, data: uint8[]): Result<int32, StorageError>   => Ok(0);
    Download(key: String): Result<uint8[], StorageError>              => Err(new StorageError("x"));
    Delete(key: String): Result<int32, StorageError>                  => Ok(0);
}

class GcpBlobStorage {
    public ctor() { }
}
impl IBlobStorage for GcpBlobStorage {
    Upload(key: String, data: uint8[]): Result<int32, StorageError>   => Ok(0);
    Download(key: String): Result<uint8[], StorageError>              => Err(new StorageError("x"));
    Delete(key: String): Result<int32, StorageError>                  => Ok(0);
}

// --- generic impl: `impl<T> ...` (impl has type_parameters) ---
interface IContainer {
    Count(): int32;
}
struct Box<T> {
    private value: T;
    public ctor(value: T) { this.value = value; }
}
impl<T> IContainer for Box<T> {
    Count(): int32 => 1;
}

// --- interface with a GENERIC METHOD signature (method_signature has type_parameters) ---
interface IMapper {
    Map<U>(input: int32): U;
}

// --- SUPERTRAITS: an interface can require (extend) other interfaces ---
//   interface_extends : EXTENDS qualified_name (',' qualified_name)* ;
interface Equatable {
    Equals(other: int32): bool;
}

// single supertrait: to be Comparable you must also be Equatable
interface Comparable extends Equatable {
    CompareTo(other: int32): int32;
}

// multiple supertraits (comma list) -- fine, interfaces carry no state
interface Ordered extends Equatable, Comparable {
    Clamp(lo: int32, hi: int32): int32;
}

// supertrait via a qualified (`::`) name
namespace Std {
    interface Encodable {
        Encode(): uint8[];
    }
}
interface Serializable extends Std::Encodable {
    ToBytes(): uint8[];
}

// --- interface + impl INSIDE a namespace (namespace_member hook) ---
namespace Graphics {
    interface IDrawable {
        Draw(): void;
    }

    struct Sprite {
        public ctor() { }
    }
    impl IDrawable for Sprite {
        Draw(): void { return; }
    }
}
