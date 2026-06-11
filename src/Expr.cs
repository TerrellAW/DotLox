namespace DotLox;

public abstract class Expr {

	internal abstract T Accept<T>(Visitor<T> visitor);

	internal interface Visitor<T> {
		internal T VisitBinaryExpr(Binary expr);
		internal T VisitGroupingExpr(Grouping expr);
		internal T VisitLiteralExpr(Literal expr);
		internal T VisitUnaryExpr(Unary expr);
	}

	internal class Binary : Expr {

		readonly Expr left;
		readonly Token opr;
		readonly Expr right;

		public Binary(Expr left, Token opr, Expr right) {
			this.left = left;
			this.opr = opr;
			this.right = right;
		}

		internal override T Accept<T>(Visitor<T> visitor) {
			return visitor.VisitBinaryExpr(this);
		}
	}

	internal class Grouping : Expr {

		readonly Expr expressions;

		public Grouping(Expr expressions) {
			this.expressions = expressions;
		}

		internal override T Accept<T>(Visitor<T> visitor) {
			return visitor.VisitGroupingExpr(this);
		}
	}

	internal class Literal : Expr {

		readonly Object value;

		public Literal(Object value) {
			this.value = value;
		}

		internal override T Accept<T>(Visitor<T> visitor) {
			return visitor.VisitLiteralExpr(this);
		}
	}

	internal class Unary : Expr {

		readonly Token opr;
		readonly Expr right;

		public Unary(Token opr, Expr right) {
			this.opr = opr;
			this.right = right;
		}

		internal override T Accept<T>(Visitor<T> visitor) {
			return visitor.VisitUnaryExpr(this);
		}
	}

}