﻿grammar Exodia;

// LEXER

// COMMENTS
SINGLE_COMMENT: '//' ~[\r\n]* -> channel(HIDDEN);
BLOCK_COMMENT: '/*' .*? '*/' -> channel(HIDDEN);

// KEYWORDS
LET: 'let' ;
RETURN: 'return' ;
FN: 'fn' ;
IF: 'if' ;
ELSE: 'else' ;
WHILE: 'while' ;
DO: 'do' ;
FOR: 'for' ;
CLASS: 'class' ;
EXTENDS: 'extends' ;
THIS: 'this' ;
SUPER: 'super';
NEW: 'new' ;

WHITESPACE: [ \t\n\r\f]+ -> skip ;
INT: [0-9]+ ;
STRING: '"' ~'"'* '"' ;
TRUE: 'true' ;
FALSE: 'false' ;

fragment
ADD: '+';
fragment
SUB: '-';
ADDITIVE_OPERATOR: ADD | SUB ;

fragment
MUL: '*';
fragment
DIV: '/';
MULTIPLICATIVE_OPERATOR: MUL | DIV ;

EQUALITY_OPERATOR: [=!]'=' ;
RELATIONAL_OPERATOR: [><]'='? ;
LOGICAL_OR: '||' ;
LOGICAL_AND: '&&' ;

SIMPLE_ASSIGNMENT_OPERATOR: [=] ;
COMPLEX_ASSIGMENT_OPERATOR: [*/+\-]'=' ;

IDENTIFIER: [a-zA-Z] [a-zA-Z1-9]* ;

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
    ;
    
class_declaration
    : CLASS identifier class_extends? statement
    ;
    
class_extends
    : EXTENDS identifier
    ;
    
iteration_statement
    : while_statement
    | do_while_statement
    | for_statement
    ;
    
for_statement
    : FOR '(' variable_statement equality_expression SEMI expression ')' statement
    ;
    
do_while_statement
    : DO statement WHILE '(' expression ')'
    ;
    
while_statement
    : WHILE '(' expression ')' statement
    ;
    
variable_statement
    : LET variable_declaration_list SEMI 
    ;

variable_declaration_list
    : variable_declaration
    | variable_declaration_list ',' variable_declaration 
    ;
   
variable_declaration
    : identifier variable_initializer? 
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
    : FN identifier '(' formal_parameter_list? ')' block_statement
    ;

formal_parameter_list
    : identifier
    | formal_parameter_list ',' identifier
    ;
    
// EXPRESSIONS

expression_statement
    : expression SEMI 
    ;
    
expression
    :  additive_expression
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
    : identifier
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
    : additive_expression
    | left=relational_expression op=RELATIONAL_OPERATOR right=additive_expression
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
    
primary_expression
    : literal
    | member_expression
    | parenthesized_expression
    | new_expression
    ;
    
parenthesized_expression
    : '(' expression ')'
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
    : INT
    ;
    
string_literal
    : STRING
    ;