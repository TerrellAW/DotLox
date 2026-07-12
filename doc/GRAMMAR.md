<div align="center">

  # DotLox Grammar

</div>

### Grammar Metasyntax Explanation

The following grammar denotes the rules in the DotLox programming language. It defines the name of the rule, followed by an `->` which is itself followed by a variety of symbols:

- `"terminal"`, individual lexemes which are tokens in the language's expressions, denoted with `" "`
- `TERMINAL`, a full caps terminal is a lexeme that can have various text representations, such as a number or a string
- `nonterminal`, which is a named reference to another rule in the grammar, is lowercase with no quotes
- `|`, denotes multiple choices, think of it as 'or'
- `(  )`, denotes a grouping
- `*`, denotes recursion, an option postfixed with this symbol can be repeated zero or more times
- `+`, similar to recursion but an option must appear atleast once
- `?`, denotes an optional value, an option postfixed with this symbol can appear zero or one time
- `;`, denotes the end of a statement

The rules are in order of descending precedence.

Example code can be found [here](../examples/).

### Grammar Rules

```
program     ->  declaration* EOF ;

declaration ->  classDecl | funDecl | varDecl | statement ;

classDecl   ->  "class" IDENTIFIER ( "<" IDENTIFIER )? "{" function* "}" ;

funDecl     ->  "fun" function ;

function    ->  IDENTIFIER "(" parameters? ")" block ;

parameters  ->  IDENTIFIER ( "," IDENTIFIER )* ;

varDecl     ->  "var" IDENTIFIER ( "=" expression )? ";" ;

statement   ->  exprStmt | forStmt | ifStmt | printStmt | returnStmt | whileStmt | block ;

exprStmt    ->  expression ";" ;

forStmt     ->  "for" "(" ( varDecl | exprStmt | ";" ) expression? ";" expression? ")" statement ;

ifStmt      ->  "if" "(" expression ")" statement ( "else" statement )? ;

printStmt   ->  "print" expression ";" ;

returnStmt  ->  "return" expression? ";" ;

whileStmt   ->  "while" "(" expression ")" statement ;

block       ->  "{" declaration* "}" ;

expression  ->  assignment ;

assignment  ->  ( call "." )? IDENTIFIER "=" assignment | logic_or ;

logic_or    ->  logic_and ( "or" logic_and )* ;

logic_and   ->  equality ( "and" equality )* ;

equality    ->  comparison ( ( "!=" | "==" ) comparison )* ;

comparison  ->  term ( ( ">" | ">=" | "<" | "<=" ) term )* ;

term        ->  factor ( ( "-" | "+" ) factor )* ;

factor      ->  unary ( ( "/" | "*" ) unary )* ;

unary       ->  ( "!" | "-" ) unary | call ;

call        ->  primary ( "(" arguments? ")" | "." IDENTIFIER )* ;

arguments   ->  expression ( "," expression )* ;

primary     ->  NUMBER | STRING | IDENTIFIER | "true" | "false" | "nil" | "this" | (" expression ")" | "super" "." IDENTIFIER ;
```
