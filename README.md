# EXODIA

# Definitions
## Variable Declaration
```csharp

global const MY_CONSTANT: int;
global let myMutatableGlobal: int;

namespace MyNamespace;

private const MY_NAMESPACED_CONSTANT: string = "THE VALUE";
protected let myNamespacedMutatableVariable: int = 5;
public 

```

## Function Declaration
```csharp
global myGlobalFunction(): int {

}

namespace MyNamespace;
```

## Primitive Types
```csharp
// we do not want true null as a value we want something like an optional from rust 
bool
int
uint
int64
uint64
string
char
higher-order functions
none | null


Optional<int>
```

## Classes
```csharp
namespace MyNamespace;

public class MyClass {
   // method definition
   public methodName(input: string): string {
   } 
}

public class MyClass {
    public MyClass(value: int) {}
    public MyClass(value: int, newVal: string) {}
}
```

## Imports / Usings
```csharp
import System.{ Console };

global globalWrite(input: string): void {
    Console.WriteLine(input);
}

globalWrite("Hello, World!");
```

## Pattern Match
```csharp
// inside of a fn or method

let result = divide(

match variable {
    Some: (value) => /* ... do the thing */,
    None: () => /* what should we do on error */
}

```