namespace DotLox;

// Implements DotLoxCallable to use class as a factory for itself
public class DotLoxClass : DotLoxCallable {
	private readonly string name;
	private readonly Dictionary<string, DotLoxFunction> methods;

	public string getName() {
		return name;
	}

	public DotLoxClass(string name, Dictionary<string, DotLoxFunction> methods) {
		this.name = name;
		this.methods = methods;
	}

	public DotLoxFunction? FindMethod(string name) {
		if (methods.ContainsKey(name))
			return methods[name];

		return null;
	}

    public override string ToString() {
        return name;
    }

	// Arity is based on constructor parameters
	public int Arity() {
		DotLoxFunction? initializer = FindMethod("init");
		if (initializer == null) return 0;
		return initializer.Arity();
	}

	// Calling a class acts as a constructor/factory method
	public object? Call(Interpreter interpreter, List<object> arguments) {
		DotLoxInstance instance = new DotLoxInstance(this);

		// User-defined constructor call
		DotLoxFunction? initializer = FindMethod("init");
		if (initializer != null) {
			initializer.Bind(instance).Call(interpreter, arguments);
		}

		return instance;
	}
}
