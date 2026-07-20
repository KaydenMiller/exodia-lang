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
    public struct String {
        private chars: Char[];
        
        public static IndexOf(inputStr: String, charToIndex: Char) : int64 {
            for (i: int32 = 0; i < chars.Length; i++) {
                if (chars[i] == charToIndex) {
                    return i;
                }
            }
            return -1;
        }
    }
    
    public struct Temperature {
        private fahrenheit: double;
        
        public ctor FromFahrenheit(f: double) {
            this.fahrenheit = f;
        }
        
        public ctor FromCelsius(c: double) {
            this.fahrenheit = c * (9/5) + 32;
        }
        
        public GetFahrenheit(): double {
            return fahrenheit;
        }
        
        public GetCelsius(): double {
            return (this.fahrenheit - 32) / 1.8;
        }
    }
    
    public struct Date {
        
    }
    
    public fn extractValueFromString(input: String) : String {
        const indexOfColon: uint16 = String.IndexOf(input, ':');
        return String.SubString(input, 0, indexOfColon);
    }
}

namespace MyPersonLib {
    public class Person {
        public Name: StandardLibrary::String;
        public Birthday: StandardLibrary::Date;
        public FavoriteTemp: StandardLibrary::Temperature;
        
        public ctor(
                name: StandardLibrary::String,
                birthday: StandardLibrary::Date,
                favoriteTemp: double) {
            this.Name = name;
            Birthday = birthday;
            FavoriteTemp = new StandardLibrary::Temperature.FromCelsius(favoriteTemp);
        }
        
        public GetName(): StandardLibrary::String {
            return this.Name;
        }
        
        public CalculateAge() : int {
            return 18;
        }
    }
}

namespace MyCustomProgram {
    public fn PersonFactory(args: StandardLibrary::String[]): MyPersonLib::Person {
        const person = new Person("Kayden", new StandardLibrary::Date("06/18/2025"));
        return person;
    }
}

// Acts as the main starting point for the application
global fn Main(args: StandardLibrary::String[]): int32 {
    const factory = MyCustomProgram::PersonFactory;
    
    const person = factory(args); 
    const personName = person.GetName();
    const personAge = person.CalculateAge();

    StandardLibrary::Console.WriteLine(personName + " is " + personAge);

    return 0;
}