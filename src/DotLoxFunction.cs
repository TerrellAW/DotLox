namespace DotLox;

public class DotLoxFunction : DotLoxCallable {
	// Function declaration
	private readonly Stmt.Function declaration;
	
	// Store environment for closures
	private readonly DotLoxEnv closure;

	// Determine if is init() method
	private readonly bool IsInit;

	// Constructor
	public DotLoxFunction(Stmt.Function declaration, DotLoxEnv closure, bool IsInit) {
		this.declaration = declaration;
		this.closure 	 = closure;
		this.IsInit 	 = IsInit;
	}

	// Bind an environment that will be used by 'this' expression
	public DotLoxFunction Bind(DotLoxInstance instance) {
		DotLoxEnv environment = new DotLoxEnv(closure);
		environment.Define("this", instance);
		return new DotLoxFunction(declaration, environment, IsInit);
	}

	// Arity implementation, takes arity from declaration's parameter list count
	public int Arity() {
		return declaration.getParameters().Count;
	}

	// Call implementation for function calls
	public Object? Call(Interpreter interpreter, List<Object> arguments) {
		// Create new scope for parameters and body
		DotLoxEnv environment = new DotLoxEnv(closure);
		
		// Store parameters in the new scope
		for (int i = 0; i < declaration.getParameters().Count; i++) {
			environment.Define(declaration.getParameters()[i].getLexeme(), arguments[i]);
		}

		// Execute code in function's block until a return statement is found
		try {
			interpreter.ExecuteBlock(declaration.getBody(), environment);
		} catch (Return returnVal) {
			// 'init()' always returns 'this'
			if (IsInit) return closure.GetAt(0, "this");

			return returnVal.getValue();
		}

		// Calling init() is the same as referencing the object
		if (IsInit) return closure.GetAt(0, "this");

		// If code execution done and no return statement, return nil
		return null;
	}

	// Output for function with no return passed to print statement
    public override string ToString()
    {
        return $"<fn {declaration.getName().getLexeme()}>";
    }
}
