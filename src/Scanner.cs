namespace DotLox;

class Scanner {
	private readonly string source;
	private readonly List<Token> tokens = new List<Token>();

	private int start 	= 0;
	private int current = 0;
	private int line 	= 1;

	// Handle reserved words
	private static readonly Dictionary<string, TokenType> keywords = new Dictionary<string, TokenType>() {
		{"and", 	TokenType.AND},
		{"class", 	TokenType.CLASS},
		{"else", 	TokenType.ELSE},
		{"false", 	TokenType.FALSE},
		{"for", 	TokenType.FOR},
		{"fun", 	TokenType.FUN},
		{"if", 		TokenType.IF},
		{"nil", 	TokenType.NIL},
		{"or", 		TokenType.OR},
		{"print", 	TokenType.PRINT},
		{"return", 	TokenType.RETURN},
		{"super", 	TokenType.SUPER},
		{"this", 	TokenType.THIS},
		{"true", 	TokenType.TRUE},
		{"var", 	TokenType.VAR},
		{"while", 	TokenType.WHILE}
	};

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

			default:
				// Alphanumeric handling
				if (IsDigit(c)) {
					HandleNumber();
				} else if (IsAlpha(c)) {
					HandleIdent();
				// Error handling
				} else {
					DotLox.HandleError(line, "Unexpected character.");
				}
				break;
		}
	}

	// Handle identifiers
	private void HandleIdent() {
		while (IsAlphaNum(Peek())) Advance();

		string text = source[start..current];

		// Get type from text, or make identifier
		if (!keywords.TryGetValue(text, out TokenType type))
			type = TokenType.IDENT;

		AddToken(type);
	}

	// Handle numbers
	private void HandleNumber() {
		// Consume entire number
		while (IsDigit(Peek())) Advance();

		// Look for and consume decimal point
		if (Peek() == '.' && IsDigit(PeekAhead())) {
			Advance();

			while (IsDigit(Peek())) Advance();
		}

		AddToken(TokenType.NUM, Double.Parse(source[start..current]));
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
			DotLox.HandleError(line, "Unterminated string.");
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
		while (!(Peek() == '*' && PeekAhead() == '/') && !IsAtEnd()) {
			if (Peek() == '\n') line++;
			Advance();
		}

		// Consume both the '*' and the '/'
		Advance();
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

	// Peek after next char
	private char PeekAhead() {
		if (current + 1 >= source.Length) return '\0';
		return source[current+1];
	}

	// Determine if a character is from the alphabet
	private bool IsAlpha(char c) {
		return 	(c >= 'a' && c <= 'z') ||
				(c >= 'A' && c <= 'Z') ||
				 c == '_';
	}

	// Determine if a character is alpha-numeric
	private bool IsAlphaNum(char c) {
		return IsAlpha(c) || IsDigit(c);
	}

	// Determine if a character is a digit
	private bool IsDigit(char c) {
		return c >= '0' && c <= '9';
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
