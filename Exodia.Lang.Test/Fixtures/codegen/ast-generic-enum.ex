// generic enums -- Option<T>/Result<T,E>, both construction forms:
//   A (explicit):     Option<int32>::Some(5)
//   C (target-typed):  const x: Option<int32> = Option::None
enum Option<T> { Some(T), None }
enum Result<T, E> { Ok(T), Err(E) }

fn main(): int32 {
    // form A -- explicit type args on the right
    const a = Option<int32>::Some(7);
    print(match a { Some(x) => x, None => 0 });        // 7

    const bd = Option<double>::None;                    // distinct instance Option$double
    print(match bd { Some(x) => 1, None => 8 });        // 8

    // form C -- type from the left annotation
    const c: Option<int32> = Option::Some(9);
    print(match c { Some(x) => x, None => 0 });         // 9

    const n: Option<int32> = Option::None;
    print(match n { Some(x) => x, None => 4 });         // 4

    // Result<int32, int32>, explicit
    const r = Result<int32, int32>::Ok(42);
    return match r { Ok(v) => v, Err(e) => e };         // 42
}
