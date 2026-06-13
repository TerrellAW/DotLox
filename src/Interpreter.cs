using DotLox.Exception;

namespace DotLox;

public class Interpreter : Expr.Visitor<Object?> {
	public Object? VisitBinaryExpr(Expr.Binary expr) {
		Object? left = Evaluate(expr.getLeft());
		Object? right = Evaluate(expr.getRight());

		switch (expr.getOpr().getType()) {
			case TokenType.BANG_EQUAL:
				return !IsEqual(left, right);
			case TokenType.EQUAL_EQUAL:
				return IsEqual(left, right);
			case TokenType.GREATER:
				return (double)left > (double)right;
			case TokenType.GREATER_EQUAL:
				return (double)left >= (double)right;
			case TokenType.LESS:
				return (double)left < (double)right;
			case TokenType.LESS_EQUAL:
				return (double)left <= (double)right;
			case TokenType.MINUS:
				CheckNumOpr(expr.getOpr(), right);
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

				// Else break
				break;
			case TokenType.SLASH:
				return (double)left / (double)right;
			case TokenType.STAR:
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
				return -(double)right;
		}

		return null;
	}

	private void CheckNumOpr(Token? opr, Object? oprnd) {
		if (oprnd is double) return;
		throw new RuntimeError(opr, "Operand must be a number.");
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

	private Object? Evaluate(Expr expr) {
		return expr.Accept(this);
	}
}
