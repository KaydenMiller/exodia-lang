// logical.ex -- short-circuit && / || with bool literals and comparisons.
// Each guard adds a distinct amount so the final sum proves which paths fired.
fn main(): int32 {
    mut score = 0;
    if (true  && (2 < 3)) { score = score + 1;    }  // true  && true  -> +1
    if (false && (2 < 3)) { score = score + 10;   }  // short-circuit false -> skip
    if (true  || (2 > 3)) { score = score + 100;  }  // short-circuit true  -> +100
    if (false || (2 > 3)) { score = score + 1000; }  // false || false -> skip
    if (false || (2 < 3)) { score = score + 4;    }  // false || true  -> +4
    return score;                                     // 1 + 100 + 4 = 105
}
