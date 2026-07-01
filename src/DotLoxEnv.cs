using DotLox.Exception;

namespace DotLox;

// Environment for variables, represents global and local scopes
public class DotLoxEnv {
	// Scope that encloses this one, null for global
	internal readonly DotLoxEnv? enclosing;
	// Variables are stored as key-value pairs
	private readonly Dictionary<string, object?> values = new Dictionary<string, object?>();

	// Getter for enclosing scope
	public DotLoxEnv? getEnclosing() {
		return enclosing;
	}
	
	// Argument-less constructor for global env
	public DotLoxEnv() {
		this.enclosing = null;
	}

	// Local env nested in another env
	public DotLoxEnv(DotLoxEnv enclosing) {
		this.enclosing = enclosing;
	}

	// Retrieve variable value from name
	public object? Get(Token name) {
		if (values.ContainsKey(name.getLexeme())) {
			return values[name.getLexeme()];
		}

		// If variable isn't found in scope, look up
		if (enclosing != null) return enclosing.Get(name);

		throw new RuntimeError(name, $"Undefined variable '{name.getLexeme()}'.");
	}

	// Assign a value to a variable
	public void Assign(Token name, Object value) {
		if (values.ContainsKey(name.getLexeme())) {
			values[name.getLexeme()] = value;
			return;
		}

		// If variable not declared in scope, look up
		if (enclosing != null) {
			enclosing.Assign(name, value);
			return;
		}

		throw new RuntimeError(name, $"Undefined variable '{name.getLexeme()}'.");
	}

	// Define a variable
	public void Define(string name, object? value) {
		values.Add(name, value);
	}

	// Get function that takes advantage of Resolver's static analysis
	public object? GetAt(int distance, string name) {
		return Ancestor(distance)?.values[name];
	}

	// Assign to a resolved variable at a specific known location
	public void AssignAt(int distance, Token name, object value) {
		Ancestor(distance)?.values[name.getLexeme()] = value;
	}

	// Find environment using distance acquired from Resolver
	internal DotLoxEnv? Ancestor(int distance) {
		DotLoxEnv? environment = this;

		for (int i = 0; i < distance; i++) {
			environment = environment?.getEnclosing();
		}

		return environment;
	}
}
