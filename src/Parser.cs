using DotLox.Exception;

namespace DotLox;

// Creates an Abstract Syntax Tree of expression and statement nodes for later interpretation
// Uses recursive descent to infer syntax
public class Parser {
	// Inputted list of tokens
	private readonly List<Token> tokens;
	// Indexing
	private int current = 0;

	// Constructor
	public Parser(List<Token> tokens) {
		this.tokens = tokens;
	}

	// Parses tokens into a list of statements
	public List<Stmt> Parse() {
		List<Stmt> statements = new List<Stmt>();

		while (!IsAtEnd()) {
			statements.Add(Declaration());
		}

		return statements;
	}

	// Begins recursive descent
	private Expr Expression() {
		return Assignment();
	}

	// Tries to pass declaration to parsing method
	private Stmt? Declaration() {
		try {
			if (Match(TokenType.VAR)) return VarDeclaration();
			return Statement();
		} catch (ParseError e) {
			Synchronize();
			return null;
		}
	}

	// Parses variable declaration
	private Stmt VarDeclaration() {
		Token name = Consume(TokenType.IDENT, "Expect variable name.");

		Expr? initializer = null;
		if (Match(TokenType.EQUAL)) {
			initializer = Expression();
		}

		Consume(TokenType.SEMICOLON, "Expect ';' after variable declaration.");
		return new Stmt.Var(name, initializer);
	}

	// Parses while statement
	private Stmt WhileStatement() {
		Consume(TokenType.LEFT_PAREN, "Expect '(' after 'while'.");
		Expr condition = Expression();
		Consume(TokenType.RIGHT_PAREN, "Expect ')' after condition.");
		Stmt body = Statement();

		return new Stmt.While(condition, body);
	}

	// Passes statement to correct parsing method
	private Stmt Statement() {
		if (Match(TokenType.FOR)) return ForStatement();
		if (Match(TokenType.IF)) return IfStatement();
		if (Match(TokenType.PRINT)) return PrintStatement();
		if (Match(TokenType.WHILE)) return WhileStatement();
		if (Match(TokenType.LEFT_BRACE)) return new Stmt.Block(Block());

		return ExpressionStatement();
	}

	// Desugars for statement into a while loop
	private Stmt ForStatement() {
		Consume(TokenType.LEFT_PAREN, "Expect '(' after 'for'.");

		Stmt? initializer;
		if (Match(TokenType.SEMICOLON)) {
			initializer = null;
		} else if (Match(TokenType.VAR)) {
			initializer = VarDeclaration();
		} else {
			initializer = ExpressionStatement();
		}

		Expr? condition = null;
		if (!Check(TokenType.SEMICOLON)) {
			condition = Expression();
		}
		Consume(TokenType.SEMICOLON, "Expect ';' after loop condition.");

		Expr? increment = null;
		if (!Check(TokenType.RIGHT_PAREN)) {
			increment = Expression();
		}
		Consume(TokenType.RIGHT_PAREN, "Expect ')' after for clauses.");
		Stmt body = Statement();

		if (increment != null) {
			body = new Stmt.Block([body, new Stmt.Expression(increment)]);
		}

		if (condition == null) condition = new Expr.Literal(true);
		body = new Stmt.While(condition, body);

		if (initializer != null) {
			body = new Stmt.Block([initializer, body]);
		}

		return body;
	}

	// Parse if statement
	private Stmt IfStatement() {
		Consume(TokenType.LEFT_PAREN, "Expect '(' after 'if'.");
		Expr condition = Expression();
		Consume(TokenType.RIGHT_PAREN, "Expect ')' after if condition.");

		Stmt thenBranch = Statement();
		Stmt? elseBranch = null;
		if (Match(TokenType.ELSE)) {
			elseBranch = Statement();
		}

		return new Stmt.If(condition, thenBranch, elseBranch);
	}

	// Parse print statement
	private Stmt PrintStatement() {
		Expr value = Expression();
		Consume(TokenType.SEMICOLON, "Expect ';' after value.");
		return new Stmt.Print(value);
	}

	// Parse expression statement
	private Stmt ExpressionStatement() {
		Expr expr = Expression();
		Consume(TokenType.SEMICOLON, "Expect ';' after expression.");
		return new Stmt.Expression(expr);
	}

	// Parse scope block
	private List<Stmt> Block() {
		List<Stmt> statements = new List<Stmt>();

		while (!Check(TokenType.RIGHT_BRACE) && !IsAtEnd()) {
			statements.Add(Declaration());
		}

		Consume(TokenType.RIGHT_BRACE, "Expect '}' after block.");
		return statements;
	}

	// Parse variable assignment
	private Expr Assignment() {
		Expr expr = Or();

		if (Match(TokenType.EQUAL)) {
			Token equals = Previous();
			Expr value = Assignment();

			if (expr is Expr.Variable) {
				Token name = ((Expr.Variable)expr).getName();
				return new Expr.Assign(name, value);
			}

			DotLox.HandleError(equals, "Invalid assignment target.");
		}

		return expr;
	}

	// Parse or
	private Expr Or() {
		Expr expr = And();

		while (Match(TokenType.OR)) {
			Token opr = Previous();
			Expr right = And();
			expr = new Expr.Logical(expr, opr, right);
		}

		return expr;
	}

	// Parse and
	private Expr And() {
		Expr expr = Equality();

		while (Match(TokenType.AND)) {
			Token opr = Previous();
			Expr right = Equality();
			expr = new Expr.Logical(expr, opr, right);
		}

		return expr;
	}

	// Parse equal and not equal
	private Expr Equality() {
		Expr expr = Comparator();
		while (Match(TokenType.BANG_EQUAL, TokenType.EQUAL_EQUAL)) {
			Token opr = Previous();
			Expr right = Comparator();
			expr = new Expr.Binary(expr, opr, right);
		}

		return expr;
	}

	// Parse comparisons
	private Expr Comparator() {
		Expr expr = Term();

		while (Match(TokenType.GREATER, TokenType.GREATER_EQUAL, TokenType.LESS, TokenType.LESS_EQUAL)) {
			Token opr = Previous();
			Expr right = Term();
			expr = new Expr.Binary(expr, opr, right);
		}

		return expr;
	}

	// Parse addition and subtraction
	private Expr Term() {
		Expr expr = Factor();

		while (Match(TokenType.MINUS, TokenType.PLUS)) {
			Token opr = Previous();
			Expr right = Factor();
			expr = new Expr.Binary(expr, opr, right);
		}

		return expr;
	}

	// Parse multiplication and division
	private Expr Factor() {
		Expr expr = Unary();

		while (Match(TokenType.SLASH, TokenType.STAR)) {
			Token opr = Previous();
			Expr right = Unary();
			expr = new Expr.Binary(expr, opr, right);
		}

		return expr;
	}

	// Parse unary expression
	private Expr Unary() {
		if (Match(TokenType.BANG, TokenType.MINUS)) {
			Token opr = Previous();
			Expr right = Unary();
			return new Expr.Unary(opr, right);
		}

		return Call();
	}

	// Parse function call
	private Expr Call() {
		Expr expr = Primary();

		while (true) {
			if (Match(TokenType.LEFT_PAREN)) {
				expr = Arguments(expr);
			} else {
				break;
			}
		}

		return expr;
	}

	// Handle call arguments
	private Expr Arguments(Expr callee) {
		List<Expr> arguments = new List<Expr>();

		// Handle empty argument list
		if (!Check(TokenType.RIGHT_PAREN)) {
			// If not empty, add expressions to arguments list
			do {
				// Limit to 255 args
				if (arguments.Count >= 255)
					// Report error without panic
					new ParseError(Peek(), "Can't have more than 255 arguments.");
				arguments.Add(Expression());
			} while (Match(TokenType.COMMA));
		}

		// Handle closing paren
		Token paren = Consume(TokenType.RIGHT_PAREN, "Expect ')' after arguments.");

		// Return function call node
		return new Expr.Call(callee, paren, arguments);
	}

	// Parse primary rule
	private Expr Primary() {
		if (Match(TokenType.FALSE)) return new Expr.Literal(false);
		if (Match(TokenType.TRUE))	return new Expr.Literal(true);
		if (Match(TokenType.NIL))	return new Expr.Literal(null);
		
		if (Match(TokenType.NUM, TokenType.STRING)) return new Expr.Literal(Previous().getLiteral());

		if (Match(TokenType.IDENT)) return new Expr.Variable(Previous());
		
		if (Match(TokenType.LEFT_PAREN)) {
			Expr expr = Expression();
			Consume(TokenType.RIGHT_PAREN, "Expect ')' after expression.");
			return new Expr.Grouping(expr);
		}

		throw new ParseError(Peek(), "Expect expression.");
	}

	private bool Match(params TokenType[] types) {
		foreach (TokenType type in types) {
			if (Check(type)) {
				Advance();
				return true;
			}
		}
		return false;
	}

	private Token Consume(TokenType type, string message) {
		if (Check(type)) return Advance();

		throw new ParseError(Peek(), message);
	}

	private bool Check(TokenType type) {
		if (IsAtEnd()) return false;
		return Peek().getType() == type;
	}

	private Token Advance() {
		if (!IsAtEnd()) current++;
		return Previous();
	}

	private bool IsAtEnd() {
		return Peek().getType() == TokenType.EOF;
	}

	private Token Peek() {
		return tokens[current];
	}

	private Token Previous() {
		return tokens[current - 1];
	}

	private void Synchronize() {
		Advance();
		while (!IsAtEnd()) {
			if (Previous().getType() == TokenType.SEMICOLON) return;

			switch (Peek().getType()) {
				case TokenType.CLASS:
				case TokenType.FOR:
				case TokenType.FUN:
				case TokenType.IF:
				case TokenType.PRINT:
				case TokenType.RETURN:
				case TokenType.VAR:
				case TokenType.WHILE:
					return;
			}

			Advance();
		}
	}
}
