using DotLox.Exception;

namespace DotLox;

// Uses recursive descent to interpret the AST created by Parser
public class Interpreter : Expr.Visitor<Object?>, Stmt.Visitor<Object?> {
	internal readonly Dictionary<Expr, int> locals = new Dictionary<Expr, int>();
	internal readonly DotLoxEnv globals = new DotLoxEnv();
	private DotLoxEnv environment = new DotLoxEnv();

	// Constructor
	public Interpreter() {
		/**
		 * Source - https://github.com/Nrosa01/CSLox/blob/master/Runtime/Src/Lox/Interpreter.cs
		 * Programmed by Nrosa01, modified by me.
		 * Retrieved 2026-06-20
		 */
		globals.Define("clock", new NativeFunction(0, (interpreter, arguments) => {
			return DateTimeOffset.Now.ToUnixTimeMilliseconds() / 1000.0;
		}));

		environment = globals;
	}

	internal void Interpret(List<Stmt> statements) {
		try {
			foreach (Stmt statement in statements) {
				Execute(statement);
			}
		} catch (RuntimeError e) {
			DotLox.HandleRuntimeError(e);
		}
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

	public Object? VisitCallExpr(Expr.Call expr) {
		Object callee = Evaluate(expr.getCallee());

		List<Object> arguments = new List<Object>();
		foreach (Expr argument in expr.getArguments()) {
			arguments.Add(Evaluate(argument));
		}

		if (!(callee is DotLoxCallable)) {
			throw new RuntimeError(expr.getParen(), "Can only call functions and classes.");
		}

		DotLoxCallable function = (DotLoxCallable)callee;
		if (arguments.Count != function.Arity()) {
			throw new RuntimeError(expr.getParen(), $"Expected {function.Arity()} arguments but got {arguments.Count}.");
		}

		return function.Call(this, arguments);
	}

	public Object? VisitGroupingExpr(Expr.Grouping expr) {
		return Evaluate(expr.getExpression());
	}

	public Object? VisitLiteralExpr(Expr.Literal expr) {
		return expr.getValue();
	}

	public Object? VisitLogicalExpr(Expr.Logical expr) {
		Object? left = Evaluate(expr.getLeft());

		if (expr.getOpr().getType() == TokenType.OR) {
			if (IsTruthy(left)) return left;
		} else {
			if (!IsTruthy(left)) return left;
		}

		return Evaluate(expr.getRight());
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

	public Object? VisitVariableExpr(Expr.Variable expr) {
		return LookupVariable(expr.getName(), expr);
	}

	private Object? LookupVariable(Token name, Expr expr) {
		// Get distance for local variable
		int distance = locals[expr];
		
		// If distance null assume its a global, which are not stored in the Dictionary
		if (distance != null) {
			return environment.GetAt(distance, name.getLexeme());
		} else {
			return globals.Get(name);
		}
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

	public Object? VisitExpressionStmt(Stmt.Expression stmt) {
		Evaluate(stmt.getExpression());
		return null;
	}

	public Object? VisitFunctionStmt(Stmt.Function stmt) {
		DotLoxFunction function = new DotLoxFunction(stmt, environment);
		environment.Define(stmt.getName().getLexeme(), function);
		return null;
	}

	public Object? VisitIfStmt(Stmt.If stmt) {
		if (IsTruthy(Evaluate(stmt.getCondition()))) {
			Execute(stmt.getThenbranch());
		} else if (stmt.getElsebranch() != null) {
			Execute(stmt.getElsebranch());
		}
		return null;
	}

	public Object? VisitPrintStmt(Stmt.Print stmt) {
		Object? value = Evaluate(stmt.getExpression());
		Console.WriteLine(Stringify(value));
		return null;
	}

	public Object? VisitReturnStmt(Stmt.Return stmt) {
		Object? value = null;
		if (stmt.getValue() != null) value = Evaluate(stmt.getValue());

		throw new Return(value);
	}

	public Object? VisitVarStmt(Stmt.Var stmt) {
		Object? value = null;
		if (stmt.getInitializer() != null) {
			value = Evaluate(stmt.getInitializer());
		}

		environment.Define(stmt.getName().getLexeme(), value);
		return null;
	}

	public Object? VisitWhileStmt(Stmt.While stmt) {
		while (IsTruthy(Evaluate(stmt.getCondition()))) {
			Execute(stmt.getBody());
		}
		return null;
	}

	public Object? VisitAssignExpr(Expr.Assign expr) {
		Object? value = Evaluate(expr.getValue());

		int distance = locals[expr];
		if (distance != null) {
			environment.AssignAt(distance, expr.getName(), value);
		} else {
			globals.Assign(expr.getName(), value);
		}

		return value;
	}

	private void Execute(Stmt stmt) {
		stmt.Accept(this);
	}

	// Tells interpreter how many layers of scoping are between a declaration and a call
	internal void Resolve(Expr expr, int depth) {
		locals[expr] = depth;
	}

	public void ExecuteBlock(List<Stmt> statements, DotLoxEnv environment) {
		DotLoxEnv previous = this.environment;

		try {
			this.environment = environment;

			foreach (Stmt statement in statements) {
				Execute(statement);
			}
		} finally {
			this.environment = previous;
		}
	}

	public Object? VisitBlockStmt(Stmt.Block stmt) {
		ExecuteBlock(stmt.getStatements(), new DotLoxEnv(environment));
		return null;
	}
}
