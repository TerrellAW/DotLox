<div align="center">

  # DotLox Grammar

</div>

### Grammar Metasyntax Explanation

The following grammar denotes the rules in the DotLox programming language. It defines the name of the rule, followed by an `->` which is itself followed by a variety of symbols:

- `"terminal"`, individual lexemes which are tokens in the language's expressions, denoted with `" "`
- `TERMINAL`, a full caps terminal is a lexeme that can have various text representations, such as a number or a string
- `nonterminal`, which is a named reference to another rule in the grammar, is lowercase with no quotes
- `|`, denotes multiple choices, think of it as 'or'
- `( option | option )`, denotes a grouping seperated with `|`, one must be chosen
- `*`, denotes recursion, an option postfixed with this symbol can be repeated zero or more times
- `+`, similar to recursion but an option must appear atleast once
- `?`, denotes an optional value, an option postfixed with this symbol can appear zero or one time
- `;`, denotes the end of a statement

The rules are in order of descending precedence.

### Grammar Rules

```
program     ->  statement* EOF ;

declaration ->  varDecl | statement ;

varDecl     ->  "var" IDENTIFIER ( "=" expression )? ";" ;

statement   ->  exprStmt | ifStmt | printStmt | whileStmt | block ;

block       ->  "{" declaration* "}" ;

whileStmt   ->  "while" "(" expression ")" statement ;

exprStmt    ->  expression ";" ;

ifStmt      ->  "if" "(" expression ")" statement ( "else" statement )? ;

printStmt   ->  "print" expression ";" ;

expression  ->  assignment ;

assignment  ->  IDENTIFIER "=" assignment | logic_or ;

logic_or    ->  logic_and ( "or" logic_and )* ;

logic_and   ->  equality ( "and" equality )* ;

equality    ->  comparison ( ( "!=" | "==" ) comparison )* ;

comparison  ->  term ( ( ">" | ">=" | "<" | "<=" ) term )* ;

term        ->  factor ( ( "-" | "+" ) factor )* ;

factor      ->  unary ( ( "/" | "*" ) unary )* ;

unary       ->  ( "!" | "-" ) unary | primary ;

primary     ->  NUMBER | STRING | "true" | "false" | "nil" | "(" expression ")" | IDENTIFIER ;
```
