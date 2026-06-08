namespace DotLox;

class Scanner {
	private readonly string source;
	private readonly List<Token> tokens = new List<Token>();

	private int start 	= 0;
	private int current = 0;
	private int line 	= 1;

	public Scanner(string source) {
		this.source = source;
	}

	public List<Token> ScanTokens() {
		while (!IsAtEnd()) {
			start = current;
			ScanToken();
		}

		tokens.Add(new Token(TokenType.EOF, "", null, line));
		return tokens;
	}

	private bool IsAtEnd() {
		return current >= source.Length;
	}

	private void ScanToken() {
		char c = Advance();

		// Match characters to tokens
		switch(c) {

			// Single character
			case '(': AddToken(TokenType.LEFT_PAREN); 	break;
			case ')': AddToken(TokenType.RIGHT_PAREN); 	break;
			case '{': AddToken(TokenType.LEFT_BRACE); 	break;
			case '}': AddToken(TokenType.RIGHT_BRACE); 	break;
			case ',': AddToken(TokenType.COMMA); 		break;
			case '.': AddToken(TokenType.DOT);			break;
			case '-': AddToken(TokenType.MINUS);		break;
			case '+': AddToken(TokenType.PLUS);			break;
			case ';': AddToken(TokenType.SEMICOLON);	break;
			case '*': AddToken(TokenType.STAR);			break;

			// Operators
			case '!':
				AddToken(Match('=') ? TokenType.BANG_EQUAL : TokenType.BANG);
				break;
			case '=':
				AddToken(Match('=') ? TokenType.EQUAL_EQUAL : TokenType.EQUAL);
				break;
			case '<':
				AddToken(Match('=') ? TokenType.LESS_EQUAL : TokenType.LESS);
				break;
			case '>':
				AddToken(Match('=') ? TokenType.GREATER_EQUAL : TokenType.GREATER);
				break;

			// Division and comments
			case '/':
				if (Match('/')) {
					// Consume comment but don't turn it into a token
					while (Peek() != '\n' && !IsAtEnd()) Advance();
				} else if (Match('*')) {
					// Handle C-style comments
					HandleComment();
				} else {
					// Turn lone slash into a token
					AddToken(TokenType.SLASH);
				}
				break;

			// Ignored characters
			case ' ':
			case '\r':
			case '\t':
				break;

			// Newline
			case '\n':
				line++;
				break;

			// Long lexemes
			case '"': HandleString(); break;

			// Error handling
			default:
				DotLox.Error(line, "Unexpected character.");
				break;
		}
	}

	// Handle string lexemes
	private void HandleString() {
		// Consume until closing quote, multi-line strings are supported
		while (Peek() != '"' && !IsAtEnd()) {
			if (Peek() == '\n') line++;
			Advance();
		}

		// Handle unterminated strings
		if (IsAtEnd()) {
			DotLox.Error(line, "Unterminated string.");
			return;
		}

		// Consume
		Advance();

		// Tokenize string and store value without quotes
		string value = source[(start+1)..(current-1)];
		AddToken(TokenType.STRING, value);
	}

	// Handle C-style comments
	private void HandleComment() {
		// Consume until closing characters, multi-line comments are supported
		while (Peek() != '*' && !Match('/') && !IsAtEnd()) {
			if (Peek() == '\n') line++;
			Advance();
			Advance();
		}

		// Consume
		Advance();
	}

	// Check second character, helper for two character tokens
	private bool Match(char expected) {
		if (IsAtEnd()) return false;
		if (source[current] != expected) return false;

		current++;
		return true;
	}

	// Peek at the next char
	private char Peek() {
		if (IsAtEnd()) return '\0';
		return source[current];
	}

	private char Advance() {
		return source[current++];
	}

	private void AddToken(TokenType type) {
		AddToken(type, null);
	}

	private void AddToken(TokenType type, Object? literal) {
		string text = source[start..current];
		tokens.Add(new Token(type, text, literal, line));
	}
}
