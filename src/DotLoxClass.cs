namespace DotLox;

// Implements DotLoxCallable to use class as a factory for itself
public class DotLoxClass : DotLoxCallable {
	private readonly string name;

	public string getName() {
		return name;
	}

	public DotLoxClass(string name) {
		this.name = name;
	}

    public override string ToString() {
        return name;
    }

	public int Arity() {
		return 0;
	}

	// Calling a class acts as a constructor/factory method
	public object? Call(Interpreter interpreter, List<object> arguments) {
		DotLoxInstance instance = new DotLoxInstance(this);
		return instance;
	}
}
