namespace DotLox;

public class DotLoxFunction : DotLoxCallable {
	// Function declaration
	private readonly Stmt.Function declaration;

	// Constructor
	public DotLoxFunction(Stmt.Function declaration) {
		this.declaration = declaration;
	}

	// Arity implementation, takes arity from declaration's parameter list count
	public int Arity() {
		return declaration.getParameters().Count;
	}

	// Call implementation for function calls
	public Object? Call(Interpreter interpreter, List<Object> arguments) {
		// Create new scope for parameters and body
		DotLoxEnv environment = new DotLoxEnv(interpreter.globals);
		
		// Store parameters in the new scope
		for (int i = 0; i < declaration.getParameters().Count; i++) {
			environment.Define(declaration.getParameters()[i].getLexeme(), arguments[i]);
		}

		// Execute code in function's block until a return statement is found
		try {
			interpreter.ExecuteBlock(declaration.getBody(), environment);
		} catch (Return returnVal) {
			return returnVal.getValue();
		}

		// If code execution done and no return statement, return nil
		return null;
	}

	// Output for function with no return passed to print statement
    public override string ToString()
    {
        return $"<fn {declaration.getName().getLexeme()}>";
    }
}
