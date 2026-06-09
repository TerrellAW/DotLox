namespace DotLox;

public abstract class Expr {

	class Binary {

		readonly Expr left;
		readonly Token opr;
		readonly Expr right;

		public Binary(Expr left, Token opr, Expr right) {
			this.left = left;
			this.opr = opr;
			this.right = right;
		}
	}

	class Grouping {

		readonly Expr expressions;

		public Grouping(Expr expressions) {
			this.expressions = expressions;
		}
	}

	class Literal {

		readonly Object value;

		public Literal(Object value) {
			this.value = value;
		}
	}

	class Unary {

		readonly Token opr;
		readonly Expr right;

		public Unary(Token opr, Expr right) {
			this.opr = opr;
			this.right = right;
		}
	}

}