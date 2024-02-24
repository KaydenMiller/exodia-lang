grammar Kaleidoscope;

WHITESPACE: [ \t\n\r\f]+ -> skip ;

DEF: 'def' ;
EXTERN: 'extern' ;

NUMBER: [0-9.]+ ;

IDENTIFIER: [a-zA-Z] [a-zA-Z0-9]* ;

fragment ADD: '+' ;
fragment SUB: '-' ;
fragment MUL: '*' ;
fragment LESS: '<' ;
OP: ADD | SUB | MUL | LESS ;

program: definition | external | expression | ';' ;

expression
    : primary binaryExpression
    | primary
    ;
    
definition: DEF prototype '{' expression '}';
external: EXTERN prototype ;

prototype: IDENTIFIER '(' IDENTIFIER* ')' ;
    
binaryExpression 
    : OP expression         #continuedBinaryExpression
    | OP primary            #finalBinaryExpression
    ;
    
identifierExpression
    : IDENTIFIER '(' (expression ',')* ')'  #callExpression
    | IDENTIFIER                            #variableExpression
    ;
    
numberExpression: NUMBER ;

parenExpression: '(' expression ')' ;
    
primary
    : identifierExpression
    | numberExpression
    | parenExpression
    ;