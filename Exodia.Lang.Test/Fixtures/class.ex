// class.ex -- reference types / entities: fields, extends, methods, constructors (ctor).

// --- qualified base + fields + ctor + methods ---
class Person extends StandardLibrary::Entity {
    private Name: String;
    public Birthday: StandardLibrary::Date;

    public ctor(name: String, birthday: StandardLibrary::Date) {
        this.Name = name;
        this.Birthday = birthday;
    }

    public GetName(): String {
        return this.Name;
    }

    public CalculateAge(): int32 {
        return 18;
    }
}

// --- empty class: member* matches ZERO members ---
class Empty {
}

// --- no `extends`: class_extends? is optional ---
private class Standalone {
    private id: int64;
}

// --- single-segment base (a qualified_name with one segment) ---
public class Admin extends User {
    level: int32;
}

// --- varied field types: array, multi-dimensional, qualified + array ---
protected class Registry {
    private entries: Person[];
    protected matrix: int32[][];
    public names: StandardLibrary::String[];
}
