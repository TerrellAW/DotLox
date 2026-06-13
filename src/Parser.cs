namespace DotLox;

public class Parser {
	private readonly List<Token> tokens;
	private int current = 0;

	public Parser(List<Token> tokens) {
		this.tokens = tokens;
	}

	public Expr? Parse() {
		try {
			return Expression();
		} catch (ParseError e) {
			return null;
		}
	}

	private Expr Expression() {
		return Equality();
	}

	private Expr Equality() {
		Expr expr = Comparator();
		while (Match(TokenType.BANG_EQUAL, TokenType.EQUAL_EQUAL)) {
			Token opr = Previous();
			Expr right = Comparator();
			expr = new Expr.Binary(expr, opr, right);
		}

		return expr;
	}

	private Expr Comparator() {
		Expr expr = Term();

		while (Match(TokenType.GREATER, TokenType.GREATER_EQUAL, TokenType.LESS, TokenType.LESS_EQUAL)) {
			Token opr = Previous();
			Expr right = Term();
			expr = new Expr.Binary(expr, opr, right);
		}

		return expr;
	}

	private Expr Term() {
		Expr expr = Factor();

		while (Match(TokenType.MINUS, TokenType.PLUS)) {
			Token opr = Previous();
			Expr right = Unary();
			expr = new Expr.Binary(expr, opr, right);
		}

		return expr;
	}

	private Expr Factor() {
		Expr expr = Unary();

		while (Match(TokenType.SLASH, TokenType.STAR)) {
			Token opr = Previous();
			Expr right = Unary();
			expr = new Expr.Binary(expr, opr, right);
		}

		return expr;
	}

	private Expr Unary() {
		if (Match(TokenType.BANG, TokenType.MINUS)) {
			Token opr = Previous();
			Expr right = Unary();
			return new Expr.Unary(opr, right);
		}

		return Primary();
	}

	private Expr Primary() {
		if (Match(TokenType.FALSE)) return new Expr.Literal(false);
		if (Match(TokenType.TRUE))	return new Expr.Literal(true);
		if (Match(TokenType.NIL))	return new Expr.Literal(null);
		
		if (Match(TokenType.NUM, TokenType.STRING)) return new Expr.Literal(Previous().getLiteral());
		
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
		if (IsAtEnd()) current++;
		return Previous();
	}

	private bool IsAtEnd() {
		return Peek().getType() == TokenType.EOF;
	}

	private Token Peek() {
		return tokens[current];
	}

	private Token Previous() {
		return tokens[current];
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

	// TODO: Make child of custom exception
	internal class ParseError : System.Exception {

		internal ParseError(Token token, string message) {
			DotLox.Error(token, message);
		}
	}
}
