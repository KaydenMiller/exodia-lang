// char.ex -- exercises the CHAR literal token and its use in expressions and patterns.
//
// Token:  CHAR : '\'' ( ~['\\\r\n] | ESCAPE ) '\'' ;
//         ESCAPE : '\\' ['"\\0nrt] ;      // \'  \"  \\  \0  \n  \r  \t
// Wired into `literal` as `char_literal`, so a char is usable anywhere a literal is.
//
// NOT covered (don't parse yet, tracked in DECISIONS §15):
//   - unicode escapes '中'  (only the raw char '中' works, not the \u form)
//   - empty '' or multi-char 'ab'  (CHAR requires exactly one char or one escape)

// --- plain single characters: letter, digit, symbol, space ---
const letter: Char = 'a';
const digit:  Char = '7';
const symbol: Char = ':';
const space:  Char = ' ';
const quote2: Char = '"';   // a bare double-quote needs no escape inside single quotes

// --- every escape in the ESCAPE fragment ---
const escQuote: Char = '\'';   // escaped single quote
const escDbl:   Char = '\"';   // escaped double quote
const escSlash: Char = '\\';   // backslash
const escNull:  Char = '\0';   // NUL
const escNL:    Char = '\n';   // newline
const escCR:    Char = '\r';   // carriage return
const escTab:   Char = '\t';   // tab

// --- Unicode BMP characters (the ~['\\\r\n] class accepts any single code point) ---
const cjk:    Char = '中';
const accent: Char = 'é';
const greek:  Char = 'Ω';

// --- char in expression positions: comparison, argument, return ---
fn isColon(c: Char): int32 {
    if (c == ':') {
        return 1;
    }
    return 0;
}

fn firstDelimiter(input: String): uint16 {
    return String.IndexOf(input, ',');
}

// --- char literals as MATCH patterns: literal, OR, and ranges ('a'..'z') ---
fn classify(c: Char): int32 {
    return match c {
        '0'..'9'          => 1,   // char range
        'a'..'z'          => 2,
        'A'..'Z'          => 3,
        ' ' | '\t' | '\n' => 4,   // OR of chars, incl. escapes
        _                 => 0,
    };
}
