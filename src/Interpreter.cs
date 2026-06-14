using DotLox.Exception;

namespace DotLox;

public class Interpreter : Expr.Visitor<Object?>, Stmt.Visitor<Object?> {
	internal void Interpret(List<Stmt> statements) {
		try {
			foreach (Stmt statement in statements) {
				Execute(statement);
			}
		} catch (RuntimeError e) {
			DotLox.HandleRuntimeError(e);
		}
	}

	public Object? VisitPrintStmt(Stmt.Print stmt) {
		Object? value = Evaluate(stmt.getExpression());
		Console.WriteLine(Stringify(value));
		return null;
	}

	public Object? VisitBinaryExpr(Expr.Binary expr) {
		Object? left = Evaluate(expr.getLeft());
		Object? right = Evaluate(expr.getRight());

		switch (expr.getOpr().getType()) {
			case TokenType.BANG_EQUAL:
				return !IsEqual(left, right);
			case TokenType.EQUAL_EQUAL:
				return IsEqual(left, right);
			case TokenType.GREATER:
				CheckNumOpr(expr.getOpr(), left, right);
				return (double)left > (double)right;
			case TokenType.GREATER_EQUAL:
				CheckNumOpr(expr.getOpr(), left, right);
				return (double)left >= (double)right;
			case TokenType.LESS:
				CheckNumOpr(expr.getOpr(), left, right);
				return (double)left < (double)right;
			case TokenType.LESS_EQUAL:
				CheckNumOpr(expr.getOpr(), left, right);
				return (double)left <= (double)right;
			case TokenType.MINUS:
				CheckNumOpr(expr.getOpr(), left, right);
				return (double)left - (double)right;
			case TokenType.PLUS:
				// Handle addition
				if (left is double && right is double) {
					return (double)left + (double)right;
				}

				// Handle concat
				if (left is string && right is string) {
					return (string)left + (string)right;
				}

				// Else fail
				throw new RuntimeError(expr.getOpr(), "Operands must be two numbers or two strings.");
			case TokenType.SLASH:
				CheckNumOpr(expr.getOpr(), left, right);
				return (double)left / (double)right;
			case TokenType.STAR:
				CheckNumOpr(expr.getOpr(), left, right);
				return (double)left * (double)right;
		}

		return null;
	}

	public Object? VisitGroupingExpr(Expr.Grouping expr) {
		return Evaluate(expr.getExpression());
	}

	public Object? VisitLiteralExpr(Expr.Literal expr) {
		return expr.getValue();
	}

	public Object? VisitUnaryExpr(Expr.Unary expr) {
		Object? right = Evaluate(expr.getRight());

		switch (expr.getOpr().getType()) {
			case TokenType.BANG:
				return !IsTruthy(right);
			case TokenType.MINUS:
				CheckNumOpr(expr.getOpr(), right);
				return -(double)right;
		}

		return null;
	}

	private void CheckNumOpr(Token? opr, Object? oprnd) {
		if (oprnd is double) return;
		throw new RuntimeError(opr, "Operand must be a number.");
	}

	private void CheckNumOpr(Token? opr, Object? left, Object? right) {
		if (left is double && right is double) return;
		throw new RuntimeError(opr, "Operands must be numbers.");
	}

	private bool IsTruthy(Object? obj) {
		if (obj == null) return false;
		if (obj is bool) return (bool)obj;
		return true;
	}

	private bool IsEqual(Object? a, Object? b) {
		if (a == null && b == null) return true;
		if (a == null) return false;

		return a.Equals(b);
	}

	private string Stringify(Object? obj) {
		if (obj == null) return "nil";

		if (obj is double) {
			string text = obj.ToString();

			if (text.EndsWith(".0")) {
				text = text.Substring(0, text.Length - 2);
			}
			return text;
		}
		return obj.ToString();
	}

	private Object? Evaluate(Expr expr) {
		return expr.Accept(this);
	}

	private void Execute(Stmt stmt) {
		stmt.Accept(this);
	}
}
