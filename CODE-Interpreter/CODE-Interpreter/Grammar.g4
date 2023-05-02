grammar Grammar;

// MAIN CODE
program: NEWLINE* 'BEGIN CODE' NEWLINE statements* exec* 'END CODE' (NEWLINE)* EOF;
//program: COMMENT* 'BEGIN CODE' NEWLINE statements+ 'END CODE' COMMENT*;
//

// one or more statement (stmt | stmt , stmts)
//statements: statement
statements: statement+;
//

// Exec (x = 123) , vardec (INT x, y =123)
statement: (vardec NEWLINE) | NEWLINE;
//statement: ((vardec | assignment | functionCall) NEWLINE) | NEWLINE;
//

exec: executes+;

executes: ((assignment | scanCall | functionCall | ifCond) NEWLINE) | NEWLINE;

// INT x or INT x, y
vardec: DATATYPE declaratorlist;
//

// x = 123 or x = y = 123 assignment: VARIABLENAME '=' value;
assignment: assignmentList '=' value;
assignmentList: value ('=' value)*;
//

//functionCall: VARIABLENAME ': ' STRINGVAL;
//functionCall: FUNCTIONNAME ': ' displayvalue+;
functionCall: DISPLAYNAME ': ' displayvalue;
scanCall: SCANNAME ': ' (scanvalue (',' scanvalue)*)?;
//

// x or y = 123
declarator: variablename | variablename '=' value;
//

// INT x or INT x, y
declaratorlist: declarator | declarator ',' declaratorlist;
//

//IF block
ifBlock: 'BEGIN IF' NEWLINE executes* 'END IF' NEWLINE*;

//IfElse
ifCond: 'IF' '(' value ')' NEWLINE ifBlock ('ELSE IF' '(' value ')' NEWLINE ifBlock)* ('ELSE' NEWLINE ifBlock)?;

constant: CHARVAL | INTEGERVAL | FLOATVAL | BOOLVAL;
variablename: VARIABLENAME;

value:
	constant				# constantExpression
	|VARIABLENAME           # valuevariablenameExpression
	| functionCall			# functionCallExpression
	| '(' value ')'         # parenthesizedExpression
	| 'NOT' value           # notExpression	
	| value multOp value	# multiplicativeExpression
	| value addOp value		# additiveExpression
	| value compareOp value	# comparisonExpression
	| value logicalOp value	# logicalOpExpression
	| logicalOp value       # logicalOpExpression
	;
	
displayvalue: 
    VARIABLENAME                        # displayvariablenameExpression
    | STRINGVAL                         # stringvalExpression
    | NEWLINEOP                         # newlineopExpression
    | '(' displayvalue ')'                  # parenthesisExpression
    | 'NOT' displayvalue                    # noExpression	
    | displayvalue multOp displayvalue	# multiExpression
    | displayvalue addOp displayvalue		# addExpression
    | displayvalue compareOp displayvalue	# compExpression
    | displayvalue logicalOp displayvalue	# logicExpression
    | logicalOp displayvalue       # logicExpression
    | displayvalue concOp displayvalue	# concatenateExpression
    ;
    
scanvalue: VARIABLENAME                 # scanvariablenameExpression;
	
multOp: '*' | '/' | '%';
addOp: '+' | '-';
compareOp: '>' | '<' | '>=' | '<=' | '==' | '<>';
logicalOp: 'OR' |'AND' |  'NOT';
concOp: '&';

NEWLINEOP: '$';
DATATYPE: 'BOOL' | 'CHAR' | 'INT' | 'FLOAT';
BOOLVAL: '"TRUE"' | '"FALSE"';
CHARVAL: '\'' ([a-z] | [A-Z] | [0-9]) '\'';
INTEGERVAL: ('-')? [1-9][0-9]*;
FLOATVAL: ('-')? [0-9]+ '.' ('-')? [0-9]+;
STRINGVAL: ('"' ~'"'* '"')
	| ('[' ~']'* ']'+);

COMMENT: '#' ~[\r\n]* -> skip;
WS: [ \t\r]+ -> skip; // Skips whitespaces
NEWLINE: [\r\n];
DISPLAYNAME: 'DISPLAY';
SCANNAME: 'SCAN';
VARIABLENAME: [_a-z][a-zA-Z0-9_]* | [a-z][a-zA-Z0-9_]*;

