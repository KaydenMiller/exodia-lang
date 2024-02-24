/** 

# OTHER
namespace   -> environment that holds library level values
class       -> heep based environment for variables
struct      -> stack based environment for variables
fn          -> a function

# VARIABLES
const       -> declares a variable at the current scope
mut         -> declares a mutable variable at the current scope

# SCOPES
global      -> can be accessed by anything
public      -> can be accessed by anything that can see the parent
internal    -> can only be accessed by sibling values of the parent
protected   -> can only be accessed by inheritors
private     -> can only be accessed by functions internal to the affected environment

# LIFETIMES
public fn add(a<'a>: uint8, b<'a>: uint8): uint8 {
    return a + b;
}

public fn add(a: uint8, b: uint8): uint8 {
    return a + b;
}

# NUMERIC TYPES
## SIMPLE
bit         -> binary 1|0
byte        -> binary 00|FF
uint8       -> unsigned int of max size 8 bits
int8        -> signed int of max size 8 bits
uint16      -> unsigned int of max size 16 bits
int16       -> signed int of max size 16 bits
uint32      -> unsigned int of max size 32 bits
int32       -> signed int of max size 32 bits
uint64      -> unsigned int of max size 64 bits
int64       -> signed int of max size 64 bits
char        -> valid UTF-8 character

## FLOATING POINT
single      -> 32  bit floating point number
double      -> 64  bit floating point number
decimal     -> 128 bit floating point number

# BUILT IN SUPPORTING TYPES
default             -> will provide the value that is set as the `default` for a given `struct|class|primitive`
string              -> valid `char` array with supporting functions
boolean             -> `bit` that is represented by `true|false` with supporting functions 
Optional<T>         -> a type that represents if a value has either one of `Some|None`
Error<T>            -> a type that represents if the state returned by the function was either `Success|Error`
Precise<float>      -> floating point number to do base10 math on the number for better accuracy but slower speed

# STANDARD LIB TYPES

*/

// comment

/* 
 block comment
*/

namespace StandardLibrary {
    public struct NameIdentifier {
    
    }
    
    public struct Value {
    
    }
    
    public extractValueFromString(input: String): String {
        const indexOfColon: uint16 = String::IndexOf(input, ':');
        return String::SubString(input, 0, indexOfColon);
    }
}