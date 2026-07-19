/**
 * result-and-option.ex
 *
 * Demonstrates the error + absence model we designed:
 *   - Result<T, E>  recoverable failure the caller MUST handle
 *   - Option<T>     a value that may be absent
 *   - match         exhaustive handling (the compiler errors if a variant is missed)
 *   - ?             propagate Err/None as an early return
 *   - throw         the rare, UNRECOVERABLE escape hatch (deliberately NOT in signatures)
 *
 * Open design choices flagged inline:
 *   (1) match-arm syntax: `Variant(binding) => body,`  -- picked here, Rust-style
 *   (2) variant resolution: bare `Some` vs qualified `Option::Some` -- bare used here
 *
 * NOTE: like sample-script.ex, this is ahead of the current grammar and will not
 * parse yet -- it specifies the target syntax.
 */

namespace StandardLibrary {

    // Option and Result are ORDINARY generic enums -- no compiler magic.
    public enum Option<T> {
        Some(T),
        None,
    }

    public enum Result<T, E> {
        Ok(T),
        Err(E),
    }

    public enum ParseError {
        Empty,
        NotANumber,
        OutOfRange,
    }

    // Recoverable failure: the RETURN TYPE forces the caller to deal with it.
    public fn parseUint16(input: String): Result<uint16, ParseError> {
        if (input.Length == 0) {
            return Err(Empty);
        }
        // ... real parsing elided; 0 is target-typed to uint16 by Ok's T ...
        return Ok(0);
    }

    // Absence: there may be no matching index.
    public fn indexOf(input: String, target: Char): Option<uint16> {
        for (i: uint16 = 0; i < input.Length; i++) {
            if (input[i] == target) {
                return Some(i);
            }
        }
        return None;
    }
}

namespace Demo {

    // Handling a Result exhaustively. Omit either arm and the compiler rejects it.
    public fn describe(input: String): String {
        match StandardLibrary::parseUint16(input) {
            Ok(value)  => return value.ToString(),
            Err(error) => return "parse failed",
        }
    }

    // Handling an Option -- same shape, absence is just data you match on.
    public fn firstColon(input: String): String {
        match StandardLibrary::indexOf(input, ':') {
            Some(i) => return "found a colon",
            None    => return "no colon present",
        }
    }

    // `?` propagates Err upward, so the function body only continues on Ok.
    public fn parseSum(a: String, b: String): Result<uint16, StandardLibrary::ParseError> {
        const first  = StandardLibrary::parseUint16(a)?;   // returns Err early if `a` fails
        const second = StandardLibrary::parseUint16(b)?;   // returns Err early if `b` fails
        return Ok(first + second);
    }

    // Opting OUT of handling: throw is the unchecked, unrecoverable escape hatch.
    // Reserved for "this should never happen", NOT for normal error flow.
    public fn mustParse(input: String): uint16 {
        match StandardLibrary::parseUint16(input) {
            Ok(value)  => return value,
            Err(error) => throw "expected a valid uint16",
        }
    }
}
