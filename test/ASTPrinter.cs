using System.Text;

namespace DotLox.Test;

public class ASTPrinter : Expr.Visitor<string> {
	public string Print(Expr expr) {
		return expr.Accept(this);
	}

	public string VisitBinaryExpr(Expr.Binary expr) {
		return Parenthesize(expr.getOpr().getLexeme(), expr.getLeft(), expr.getRight());
	}

	public string VisitGroupingExpr(Expr.Grouping expr) {
		return Parenthesize("group", expr.getExpressions());
	}

	public string VisitLiteralExpr(Expr.Literal expr) {
		return (expr.getValue() == null) ? "nil" : expr.getValue().ToString();
	}

	public string VisitUnaryExpr(Expr.Unary expr) {
		return Parenthesize(expr.getOpr().getLexeme(), expr.getRight());
	}

	private String Parenthesize(string name, params Expr[] exprs) {
		StringBuilder builder = new StringBuilder();

		builder.Append("(").Append(name);
		foreach (Expr expr in exprs) {
			builder.Append(" ");
			builder.Append(expr.Accept(this));
		}
		builder.Append(")");

		return builder.ToString();
	}
}
