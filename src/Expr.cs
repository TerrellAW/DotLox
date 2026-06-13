namespace DotLox;

public abstract class Expr {

	public abstract T Accept<T>(Visitor<T> visitor);

	public interface Visitor<T> {
		public T VisitBinaryExpr(Binary expr);
		public T VisitGroupingExpr(Grouping expr);
		public T VisitLiteralExpr(Literal expr);
		public T VisitUnaryExpr(Unary expr);
	}

	public class Binary : Expr {

		readonly Expr left;
		readonly Token opr;
		readonly Expr right;

		public Expr getLeft() {
			return left;
		}

		public Token getOpr() {
			return opr;
		}

		public Expr getRight() {
			return right;
		}

		public Binary(Expr left, Token opr, Expr right) {
			this.left = left;
			this.opr = opr;
			this.right = right;
		}

		public override T Accept<T>(Visitor<T> visitor) {
			return visitor.VisitBinaryExpr(this);
		}
	}

	public class Grouping : Expr {

		readonly Expr expressions;

		public Expr getExpressions() {
			return expressions;
		}

		public Grouping(Expr expressions) {
			this.expressions = expressions;
		}

		public override T Accept<T>(Visitor<T> visitor) {
			return visitor.VisitGroupingExpr(this);
		}
	}

	public class Literal : Expr {

		readonly Object? value;

		public Object? getValue() {
			return value;
		}

		public Literal(Object? value) {
			this.value = value;
		}

		public override T Accept<T>(Visitor<T> visitor) {
			return visitor.VisitLiteralExpr(this);
		}
	}

	public class Unary : Expr {

		readonly Token opr;
		readonly Expr right;

		public Token getOpr() {
			return opr;
		}

		public Expr getRight() {
			return right;
		}

		public Unary(Token opr, Expr right) {
			this.opr = opr;
			this.right = right;
		}

		public override T Accept<T>(Visitor<T> visitor) {
			return visitor.VisitUnaryExpr(this);
		}
	}

}