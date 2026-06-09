namespace DotLox.Expressions;

class Binary : Expr {
	readonly Expr 	left;
	readonly Token 	opr;
	readonly Expr 	right;

	public Binary(Expr left, Token opr, Expr right) {
		this.left 	= left;
		this.opr 	= opr;
		this.right	= right;
	}
}
