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
const letter: char = 'a';
const digit:  char = '7';
const symbol: char = ':';
const space:  char = ' ';
const quote2: char = '"';   // a bare double-quote needs no escape inside single quotes

// --- every escape in the ESCAPE fragment ---
const escQuote: char = '\'';   // escaped single quote
const escDbl:   char = '\"';   // escaped double quote
const escSlash: char = '\\';   // backslash
const escNull:  char = '\0';   // NUL
const escNL:    char = '\n';   // newline
const escCR:    char = '\r';   // carriage return
const escTab:   char = '\t';   // tab

// --- Unicode BMP characters (the ~['\\\r\n] class accepts any single code point) ---
const cjk:    char = '中';
const accent: char = 'é';
const greek:  char = 'Ω';

// --- char in expression positions: comparison, argument, return ---
fn isColon(c: char): int32 {
    if (c == ':') {
        return 1;
    }
    return 0;
}

fn firstDelimiter(input: String): uint16 {
    return String.IndexOf(input, ',');
}

// --- char literals as MATCH patterns: literal, OR, and ranges ('a'..'z') ---
fn classify(c: char): int32 {
    return match c {
        '0'..'9'          => 1,   // char range
        'a'..'z'          => 2,
        'A'..'Z'          => 3,
        ' ' | '\t' | '\n' => 4,   // OR of chars, incl. escapes
        _                 => 0,
    };
}
