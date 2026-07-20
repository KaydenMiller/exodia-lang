// unicode-ident.ex -- exercises the full-Unicode IDENTIFIER rule.
// The ASCII-only rule would still parse the plain names, so the point of this
// file is the NON-ASCII identifiers: if these bind as identifiers, the
// \p{...} categories in IdentifierStart / IdentifierPart are wired correctly.

// --- \p{Ll} / \p{Lu}: baseline ASCII letters still work ---
const count = 1;
const Total = 2;

// --- \p{Mn}: accented letters (base letter + combining mark, or precomposed) ---
const café  = 3;
const naïve = 4;

// --- \p{Lo}: cased-less scripts (CJK, Hebrew, Arabic) ---
const 変数    = 5;    // Japanese
const 变量    = 6;    // Chinese
const переменная = 7; // Cyrillic (\p{Ll} outside ASCII)

// --- \p{Nl}: letter-numbers are valid even as a START character ---
const Ⅲ = 8;         // Roman numeral three (U+2162)

// --- \p{Nd} in CONTINUE position: digits after the first char ---
const value2 = 9;
const item_42 = 10;

// --- \p{Pc}: underscore as a leading start char, and as a connector ---
const _skip   = 11;
const my_name = 12;

// --- '@' verbatim prefix: use a keyword as an identifier ---
const @class  = 13;
const @match  = 14;

// --- mixed: script + digits + underscore in one name ---
const 変数_2 = 15;

// --- a function using unicode names, to prove identifiers work past `const` ---
fn Σ(θ: int32): int32 {
    mut résultat = θ;
    résultat = résultat + 1;
    return résultat;
}
