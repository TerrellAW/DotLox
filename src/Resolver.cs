namespace DotLox;

public class Resolver : Expr.Visitor<object?>, Stmt.Visitor<object?> {
	// Use interpreter to resolve, but don't execute code
	private readonly Interpreter interpreter;

	// Stack of scopes
	private readonly Stack<Dictionary<string, bool>> scopes = new Stack<Dictionary<string, bool>>();

	public Resolver(Interpreter interpreter) {
		this.interpreter = interpreter;
	}

	// Start with blocks as they create scopes
	public object? VisitBlockStmt(Stmt.Block stmt) {
		// Begin a new scope
		BeginScope();

		// Resolve statements in block
		Resolve(stmt.getStatements());

		// Discard the scope
		EndScope();
		return null;
	}

	public object? VisitExpressionStmt(Stmt.Expression stmt) {
		Resolve(stmt.getExpression());
		return null;
	}

	public object? VisitFunctionStmt(Stmt.Function stmt) {
		Declare(stmt.getName());
		Define(stmt.getName());
		ResolveFunction(stmt);
		return null;
	}

	public object? VisitIfStmt(Stmt.If stmt) {
		// Analyse both branches
		Resolve(stmt.getCondition());
		Resolve(stmt.getThenbranch());

		if (stmt.getElsebranch() != null) Resolve(stmt.getElsebranch());
		return null;
	}

	public object? VisitPrintStmt(Stmt.Print stmt) {
		Resolve(stmt.getExpression());
		return null;
	}

	public object? VisitReturnStmt(Stmt.Return stmt) {
		if (stmt.getValue() != null) {
			Resolve(stmt.getValue());
		}

		return null;
	}

	public object? VisitVarStmt(Stmt.Var stmt) {
		// Declare variable
		Declare(stmt.getName());

		// Resolve initializer if not null
		if (stmt.getInitializer() != null) Resolve(stmt.getInitializer());

		// Define variable
		Define(stmt.getName());
		return null;
	}

	public object? VisitWhileStmt(Stmt.While stmt) {
		// Analyse loop exactly once
		Resolve(stmt.getCondition());
		Resolve(stmt.getBody());
		return null;
	}

	public object? VisitAssignExpr(Expr.Assign expr) {
		Resolve(expr.getValue());
		ResolveLocal(expr, expr.getName());
		return null;
	}

	public object? VisitBinaryExpr(Expr.Binary expr) {
		Resolve(expr.getLeft());
		Resolve(expr.getRight());
		return null;
	}

	public object? VisitCallExpr(Expr.Call expr) {
		Resolve(expr.getCallee());

		foreach (Expr argument in expr.getArguments()) {
			Resolve(argument);
		}

		return null;
	}

	public object? VisitGroupingExpr(Expr.Grouping expr) {
		Resolve(expr.getExpression());
		return null;
	}

	public object? VisitLiteralExpr(Expr.Literal expr) {
		return null;
	}

	public object? VisitLogicalExpr(Expr.Logical expr) {
		Resolve(expr.getLeft());
		Resolve(expr.getRight());
		return null;
	}

	public object? VisitUnaryExpr(Expr.Unary expr) {
		Resolve(expr.getRight());
		return null;
	}

	public object? VisitVariableExpr(Expr.Variable expr) {
		// Handle uninitialized variable being assigned to itself
		if (!(scopes.Count == 0) && (scopes.Peek().TryGetValue(expr.getName().getLexeme(), out bool initialized) && !initialized)) {
			DotLox.HandleError(expr.getName(), "Can't read local variable in its own initializer.");
		}

		// Read scope map to resolve variable expression
		ResolveLocal(expr, expr.getName());
		return null;
	}

	// Walk list of statements to resolve
	internal void Resolve(List<Stmt> statements) {
		foreach (Stmt statement in statements) {
			// Call overloaded function to handle individual statement
			Resolve(statement);
		}
	}

	// Resolves individual statement
	private void Resolve(Stmt stmt) {
		stmt.Accept(this);
	}

	// Resolves individual expression
	private void Resolve(Expr expr) {
		expr.Accept(this);
	}

	private void ResolveFunction(Stmt.Function function) {
		BeginScope();

		// Handle function parameters
		foreach (Token param in function.getParameters()) {
			Declare(param);
			Define(param);
		}

		// Handle function body
		Resolve(function.getBody());

		EndScope();
	}

	// Create a new scope and add it to stack
	private void BeginScope() {
		scopes.Push(new Dictionary<string, bool>());
	}

	// Exit scope and discard it
	private void EndScope() {
		scopes.Pop();
	}

	// Declare variable so it shadows any outer one with the same name
	private void Declare(Token name) {
		// Global variables don't shadow anything
		if (scopes.Count == 0 ) return;

		// Declare variable and mark as not ready
		Dictionary<string, bool> scope = scopes.Peek();
		scope[name.getLexeme()] = false;
	}

	// Define a variable to make it ready for use
	private void Define(Token name) {
		// Global variables don't shadow anything
		if (scopes.Count == 0 ) return;

		// Mark as ready for use (initialized)
		scopes.Peek()[name.getLexeme()] = true;
	}

	// Resolve local variable assignment
	private void ResolveLocal(Expr expr, Token name) {
		// Decrement through scopes
		for (int i = scopes.Count - 1; i >= 0; i--) {
			// Pass amount of layers since declaration to interpreter
			if (scopes.ElementAt(i).ContainsKey(name.getLexeme())) {
				interpreter.Resolve(expr, scopes.Count - 1 - i);
				return;
			}
		}
	}
}
