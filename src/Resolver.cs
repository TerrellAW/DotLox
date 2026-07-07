namespace DotLox;

public class Resolver : Expr.Visitor<object?>, Stmt.Visitor<object?> {
	// Use interpreter to resolve, but don't execute code
	private readonly Interpreter interpreter;

	// Stack of scopes
	private readonly Stack<Dictionary<string, bool>> scopes = new Stack<Dictionary<string, bool>>();

	// Function tracker
	private FunctionType currentFunction = FunctionType.NONE;

	private enum FunctionType {
		NONE,
		FUNCTION,
		INITIALIZER,
		METHOD
	}

	private enum ClassType {
		NONE,
		CLASS
	}

	private ClassType currentClass = ClassType.NONE;

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

	// Prevent infinite loop in malformed code
	public object? VisitClassStmt(Stmt.Class stmt) {
		ClassType enclosingClass = currentClass;
		currentClass = ClassType.CLASS;

		Declare(stmt.getName());
		Define(stmt.getName());

		// Create scope for current state of object
		// Will be used when 'this.property' is used
		BeginScope();
		scopes.Peek()["this"] = true;

		// Resolve methods
		foreach (Stmt.Function method in stmt.getMethods()) {
			FunctionType decl = FunctionType.METHOD;

			// Handle user-defined constructors
			if (method.getName().getLexeme().Equals("init"))
				decl = FunctionType.INITIALIZER;

			ResolveFunction(method, decl);
		}

		EndScope();

		currentClass = enclosingClass;
		return null;
	}

	public object? VisitExpressionStmt(Stmt.Expression stmt) {
		Resolve(stmt.getExpression());
		return null;
	}

	public object? VisitFunctionStmt(Stmt.Function stmt) {
		Declare(stmt.getName());
		Define(stmt.getName());
		ResolveFunction(stmt, FunctionType.FUNCTION);
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
		if (currentFunction == FunctionType.NONE)
			DotLox.HandleError(stmt.getKeyword(), "Can't return from top-level code.");

		if (stmt.getValue() != null) {
			// Disallow returning from user-defined constructors
			if (currentFunction == FunctionType.INITIALIZER) {
				DotLox.HandleError(stmt.getKeyword(), "Can't return a value from an initializer.");
			}

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

	public object? VisitGetExpr(Expr.Get expr) {
		Resolve(expr.getObj());
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

	public object? VisitSetExpr(Expr.Set expr) {
		Resolve(expr.getValue());
		Resolve(expr.getObj());
		return null;
	}

	public object? VisitThisExpr(Expr.This expr) {
		// Ensure 'this' expression is only used inside a class
		if (currentClass == ClassType.NONE) {
			DotLox.HandleError(expr.getKeyword(), "Can't use 'this' outside of a class.");
			return null;
		}

		ResolveLocal(expr, expr.getKeyword());
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

	private void ResolveFunction(Stmt.Function function, FunctionType type) {
		FunctionType enclosingFunction = currentFunction;
		currentFunction = type;

		BeginScope();

		// Handle function parameters
		foreach (Token param in function.getParameters()) {
			Declare(param);
			Define(param);
		}

		// Handle function body
		Resolve(function.getBody());

		EndScope();
		currentFunction = enclosingFunction;
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
		if (scope.ContainsKey(name.getLexeme())) {
			DotLox.HandleError(name, "Already a variable with this name in this scope.");
		}

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
		// Increment through scopes
		for (int i = 0; i < scopes.Count; i++) {
			// Pass amount of layers since declaration to interpreter
			if (scopes.ElementAt(i).ContainsKey(name.getLexeme())) {
				interpreter.Resolve(expr, i);
				return;
			}
		}
	}
}
