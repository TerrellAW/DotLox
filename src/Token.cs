namespace DotLox;

public class Token {
	readonly TokenType 	type;
	readonly string 	lexeme;
	readonly Object? 	literal;
	readonly int 		line;

	public string getLexeme() {
		return lexeme;
	}

	public Token(TokenType type, string lexeme, Object? literal, int line) {
		this.type 		= type;
		this.lexeme 	= lexeme;
		this.literal 	= literal;
		this.line		= line;
	}

	public override string ToString() {
		return $"type {lexeme} {literal}";
	}
}

public enum TokenType {
	// Single character tokens
	LEFT_PAREN, RIGHT_PAREN, LEFT_BRACE, RIGHT_BRACE, COMMA, DOT, MINUS, PLUS, SEMICOLON, SLASH, STAR,

	// Conditional tokens
	BANG, BANG_EQUAL, EQUAL, EQUAL_EQUAL, GREATER, GREATER_EQUAL, LESS, LESS_EQUAL,

	// Literals
	IDENT, STRING, NUM,

	// Keywords
	AND, CLASS, ELSE, FALSE, FUN, FOR, IF, NIL, OR, PRINT, RETURN, SUPER, THIS, TRUE, VAR, WHILE,

	// End of file
	EOF
}
