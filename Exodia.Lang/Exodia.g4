grammar Exodia;

// LEXER

// COMMENTS
SINGLE_COMMENT: '//' ~[\r\n]* -> channel(HIDDEN);
BLOCK_COMMENT: '/*' .*? '*/' -> channel(HIDDEN);

// KEYWORDS
MUT: 'mut' ;
CONST: 'const' ;
RETURN: 'return' ;
FN: 'fn' ;
IF: 'if' ;
ELSE: 'else' ;
WHILE: 'while' ;
DO: 'do' ;
FOR: 'for' ;
STRUCT: 'struct' ;
CLASS: 'class' ;
ENUM: 'enum' ;
NAMESPACE: 'namespace' ;
EXTENDS: 'extends' ;
THIS: 'this' ;
SUPER: 'super';
NEW: 'new' ;
MATCH: 'match' ;
WHEN: 'when' ;

CONSTRUCTOR: 'ctor';

PUBLIC: 'public' ;
PRIVATE: 'private' ;
PROTECTED: 'protected' ;
INTERNAL: 'internal' ;
GLOBAL: 'global' ;
STATIC: 'static' ;

WHITESPACE: [ \t\n\r\f]+ -> skip ;

fragment DIGITS : [0-9] ( [0-9_]* [0-9] )? ;
INTEGER: DIGITS INTEGER_SUFFIX?;
FLOAT: DIGITS '.' DIGITS ([eE] [+-]? DIGITS )? FLOAT_SUFFIX?;
fragment INTEGER_SUFFIX : [iu] ('8'|'16'|'32'|'64');    // 5i8, 42u32
fragment FLOAT_SUFFIX : 'f' | 'd' | 'm' ;               // 1.5f, 1.5m

STRING: '"' ~'"'* '"' ;
TRUE: 'true' ;
FALSE: 'false' ;

fragment ADD: '+';
fragment SUB: '-';
ADDITIVE_OPERATOR: ADD | SUB ;

fragment MUL: '*';
fragment DIV: '/';
MULTIPLICATIVE_OPERATOR: MUL | DIV ;

LT: '<' ; 
GT: '>' ;
LE: '<=' ;
GE: '>=' ;

FATARROW: '=>' ;
PIPE: '|' ;
DOTDOT: '..' ;

COLONCOLON: '::' ;

COLON: ':' ;
EQUALITY_OPERATOR: [=!]'=' ;
LOGICAL_OR: '||' ;
LOGICAL_AND: '&&' ;

SIMPLE_ASSIGNMENT_OPERATOR: [=] ;
COMPLEX_ASSIGMENT_OPERATOR: [*/+\-]'=' ;

IDENTIFIER
    : '@'? IdentifierStart IdentifierPart*
    ;

fragment IdentifierStart
    : [\p{L}\p{Nl}] // letters + letter-numbers
    | '_'
    ;
    
fragment IdentifierPart
    : [\p{L}\p{Nl}\p{Mn}\p{Mc}\p{Nd}\p{Pc}\p{Cf}]
    ;

SEMI: ';';

// PARSER

program: statement* EOF; // THE ? is so you can have an empty file

// STATEMENTS

statement
    : expression_statement 
    | empty_statement
    | block_statement
    | function_declaration
    | return_statement
    | variable_statement
    | if_statement
    | iteration_statement
    | class_declaration
    | struct_declaration
    | enum_declaration
    | namespace_declaration
    ;
    
accessability_modifier
    : PUBLIC | PRIVATE | INTERNAL | PROTECTED | GLOBAL | STATIC
    ;
    
mut_flag
    : MUT
    ;
    
namespace_member
    : struct_declaration
    | class_declaration
    | function_declaration
    | enum_declaration
    | namespace_declaration
    ;
    
namespace_declaration
    : NAMESPACE qualified_name '{' namespace_member* '}'
    ;
    
member_kind
    : mut_flag? field_declaration
    | method_declaration
    | constructor_declaration
    ;
    
member
    : accessability_modifier* member_kind
    ;
    
method_declaration
    : identifier type_parameters? '(' formal_parameter_list? ')' COLON type block_statement
    ;
    
constructor_declaration
    : CONSTRUCTOR identifier? '(' formal_parameter_list? ')' block_statement
    ;
   
field_declaration
    : identifier COLON type SEMI
    ;
    
class_declaration
    : accessability_modifier* CLASS identifier type_parameters? class_extends? '{' member* '}'
    ;

enum_declaration
    : accessability_modifier* ENUM identifier type_parameters? '{' enum_variant_list? '}'
    ;
    
enum_variant_list
    : enum_variant (',' enum_variant)* ','?
    ;   
    
enum_variant
    : identifier enum_variant_payload?
    ;
    
enum_variant_payload
    : '(' type (',' type)* ')'
    ;
    
struct_declaration
    : accessability_modifier* STRUCT identifier type_parameters? '{' member* '}'
    ;
    
class_extends
    : EXTENDS qualified_name
    ;
    
iteration_statement
    : while_statement
    | do_while_statement
    | for_statement
    ;
    
for_statement
    : FOR '(' variable_statement expression SEMI expression ')' statement
    ;
    
do_while_statement
    : DO statement WHILE '(' expression ')'
    ;
    
while_statement
    : WHILE '(' expression ')' statement
    ;
    
variable_statement
    : (CONST | MUT) variable_declaration_list SEMI 
    ;
    
variable_declaration
    : identifier (COLON type)? variable_initializer? 
    ;

variable_declaration_list
    : variable_declaration
    | variable_declaration_list ',' variable_declaration 
    ;

variable_initializer
    : SIMPLE_ASSIGNMENT_OPERATOR assignment_expression
    ;
    
if_statement
    : IF '(' expression ')' statement
    | IF '(' expression ')' statement ELSE statement
    ;

empty_statement
    : SEMI 
    ;

return_statement
    : RETURN expression? SEMI 
    ;
    
block_statement
    : '{' statement* '}'
    ;
    
// FUNCTIONS

function_declaration
    : accessability_modifier* FN identifier type_parameters? '(' formal_parameter_list? ')' COLON type block_statement
    ;
   
formal_parameter
    : identifier COLON type
    ;

formal_parameter_list
    : formal_parameter (',' formal_parameter)* 
    ;
    
// EXPRESSIONS

expression_statement
    : expression SEMI 
    ;
    
expression
    :  assignment_expression
    ;
    
assignment_expression
    : logical_OR_expression
    | left_hand_side_expression assignment_operator assignment_expression
    ;
    
assignment_operator
    : SIMPLE_ASSIGNMENT_OPERATOR
    | COMPLEX_ASSIGMENT_OPERATOR
    ;
    
left_hand_side_expression
    : member_expression
    ;
    
member_expression
    : qualified_name
    | this_expression
    | member_expression '.' identifier
    | member_expression '[' expression ']'
    ;
    
this_expression
    : THIS
    ;
    
identifier
    : IDENTIFIER 
    ;
    
logical_OR_expression
    : logical_AND_expression
    | left=logical_OR_expression op=LOGICAL_OR right=logical_AND_expression
    ;
    
logical_AND_expression
    : equality_expression
    | left=logical_AND_expression op=LOGICAL_AND right=equality_expression
    ;
    
equality_expression
    : relational_expression
    | left=equality_expression op=EQUALITY_OPERATOR right=relational_expression 
    ;
    
relational_expression
    : shift_expression
    | left=relational_expression op=(LT | GT | LE | GE) right=shift_expression
    ;
    
shift_expression
    : additive_expression
    | left=shift_expression op=shift_operator right=additive_expression
    ;
    
shift_operator
    : LT LT 
    | GT GT
    ;
    
additive_expression
    : multiplicative_expression 
    | left=additive_expression op=ADDITIVE_OPERATOR right=multiplicative_expression
    ;
    
multiplicative_expression
    : unary_expression 
    | left=multiplicative_expression op=MULTIPLICATIVE_OPERATOR right=unary_expression 
    ;
    
unary_expression
    : primary_expression 
    | call_expression
    | op=ADDITIVE_OPERATOR unary_expression
    ;
   
call_expression
    : callee args=arguments                      
    | super args=arguments                       
    | call_expression args=arguments             
    ;
    
super
    : SUPER
    ;
    
callee
    : lhse=left_hand_side_expression
    ;
    
arguments
    : '(' argument_list? ')'
    ;

argument_list
    : assignment_expression 
    | argument_list ',' assignment_expression
    ;
    
new_expression
    : NEW exp=member_expression args=arguments
    ;
    
match_expression
    : MATCH expression '{' match_arm (',' match_arm)* ','? '}'
    ;
    
match_arm
    : pattern (WHEN expression)? FATARROW arm_body
    ;
    
arm_body
    : expression
    | block_statement
    ;
    
pattern
    : primary_pattern (PIPE primary_pattern)*
    ;
    
primary_pattern
    : qualified_name pattern_payload?
    | literal (DOTDOT literal)?
    | '_'
    ;
    
pattern_payload
    : '(' pattern (',' pattern)* ')'
    ;
    
primary_expression
    : literal
    | member_expression
    | parenthesized_expression
    | new_expression
    | match_expression
    ;
    
parenthesized_expression
    : '(' expression ')'
    ;
    
qualified_name
    : identifier (COLONCOLON identifier)* 
    ;
    
type
    : qualified_name type_arguments? ('[' ']')* 
    ;

type_arguments
    : LT type (',' type)* GT
    ;
    
type_parameters
    : LT identifier (',' identifier)* GT
    ;
    
// LITERALS

literal
    : numeric_literal       #atom
    | string_literal        #atom
    | true_literal          #atom
    | false_literal         #atom
    ;
    
true_literal
    : TRUE
    ;
    
false_literal
    : FALSE
    ;
    
numeric_literal
    : INTEGER
    | FLOAT
    ;
    
string_literal
    : STRING
    ;