namespace DotLox;

public abstract class Expr {
	class Binary {
		public Binary(Expr left, Token opr, Expr right) {
			this.left = left;
			this.opr = opr;
			this.right = right;
		}

		readonly Expr left;
		readonly Token opr;
		readonly Expr right;
	}
	class Grouping {
		public Grouping(Expr expressions) {
			this.expressions = expressions;
		}

		readonly Expr expressions;
	}
	class Literal {
		public Literal(Object value) {
			this.value = value;
		}

		readonly Object value;
	}
	class Unary {
		public Unary(Token opr, Expr right) {
			this.opr = opr;
			this.right = right;
		}

		readonly Token opr;
		readonly Expr right;
	}
}