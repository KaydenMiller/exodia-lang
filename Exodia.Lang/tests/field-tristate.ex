/**
 * field-tristate.ex
 *
 * Demonstrates Field<T>: the three-state value for JSON / DB partial updates.
 *
 *   Absent    -> caller did not mention the field   (JSON key missing;  don't touch the column)
 *   Null      -> caller explicitly cleared it       (JSON `null`;        SET column = NULL)
 *   Value(v)  -> caller set a concrete value         (JSON value present; SET column = v)
 *
 * KEY POINT: Field<T> is an ORDINARY enum. Nothing in the language treats
 * `Absent` specially. Only the update-builder below gives `Absent` its "skip"
 * meaning -- and that is just a normal match arm that emits nothing.
 *
 * NOTE: ahead of the current grammar (like sample-script.ex). Struct-literal
 * syntax in `examplePatch` is illustrative and not yet designed.
 */

namespace StandardLibrary {
    public enum Field<T> {
        Absent,
        Null,
        Value(T),
    }
}

namespace PersonApi {

    // A PATCH body: every field is a Field<T>, so each can independently be
    // absent, explicitly null, or set. This is what C#'s single `null` cannot express.
    public struct UpdatePersonRequest {
        public Name:     StandardLibrary::Field<String>;
        public Nickname: StandardLibrary::Field<String>;
        public Age:      StandardLibrary::Field<uint16>;
    }

    // Turn a patch into SQL SET clauses.
    // This is the ONLY place `Absent` means "skip" -- an ordinary match arm.
    public fn buildSetClauses(patch: UpdatePersonRequest): String[] {
        mut clauses: String[] = [];

        match patch.Name {
            Absent   => { },                                      // leave the column untouched
            Null     => clauses.Append("name = NULL"),
            Value(v) => clauses.Append("name = " + quote(v)),
        }

        match patch.Nickname {
            Absent   => { },
            Null     => clauses.Append("nickname = NULL"),
            Value(v) => clauses.Append("nickname = " + quote(v)),
        }

        match patch.Age {
            Absent   => { },
            Null     => clauses.Append("age = NULL"),
            Value(v) => clauses.Append("age = " + v.ToString()),
        }

        return clauses;
    }

    // Constructing a patch: set the name, explicitly clear the nickname,
    // and leave age completely alone. Three different intentions, all expressible.
    public fn examplePatch(): UpdatePersonRequest {
        return new UpdatePersonRequest {
            Name:     Value("Kayden"),
            Nickname: Null,
            Age:      Absent,
        };
    }
}
