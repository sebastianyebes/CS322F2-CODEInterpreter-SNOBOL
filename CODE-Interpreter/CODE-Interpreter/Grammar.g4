grammar Grammar;

// MAIN CODE
program: NEWLINE* 'BEGIN CODE' NEWLINE statements* 'END CODE' (NEWLINE)* EOF;
//program: COMMENT* 'BEGIN CODE' NEWLINE statements+ 'END CODE' COMMENT*;
//

// one or more statement (stmt | stmt , stmts)
statements: statement+;
//


// Exec (x = 123) , vardec (INT x, y =123)
statement: ((vardec | assignment | functionCall) NEWLINE) | NEWLINE;
//

// INT x or INT x, y
vardec: DATATYPE declaratorlist;
//

// x = 123 or x = y = 123 assignment: VARIABLENAME '=' value;
assignment: assignmentList '=' value;
assignmentList: value ('=' value)*;
//

//functionCall: VARIABLENAME ': ' STRINGVAL;
functionCall: FUNCTIONNAME ': ' displayvalue+;
//

// x or y = 123
declarator: variablename | variablename '=' value;
//

// INT x or INT x, y
declaratorlist: declarator | declarator ',' declaratorlist;
//

constant: CHARVAL | INTEGERVAL | FLOATVAL | BOOLVAL;
variablename: VARIABLENAME;

value:
	constant				# constantExpression
	|VARIABLENAME           # valuevariablenameExpression
	| functionCall			# functionCallExpression
	| value compareOp value	# comparisonExpression
	| value logicalOp value	# logicalOpExpression
	| value multOp value	# multiplicativeExpression
	| value addOp value		# additiveExpression
	;
	
displayvalue: 
    VARIABLENAME            # displayvariablenameExpression
    | STRINGVAL             # stringvalExpression
    | NEWLINEOP             # newlineopExpression
    | displayvalue concOp displayvalue	# concatenateExpression
    ;
	
multOp: '*' | '/' | '%';
addOp: '+' | '-';
compareOp: '>' | '<' | '>=' | '<=' | '==' | '<>';
logicalOp: 'AND' | 'OR' | 'NOT';
concOp: '&';
assgnOp: '=';

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
FUNCTIONNAME: 'DISPLAY' | 'SCAN';
VARIABLENAME: [_a-z][a-zA-Z0-9_]* | [a-z][a-zA-Z0-9_]*;

