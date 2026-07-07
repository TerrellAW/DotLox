using DotLox.Exception;

namespace DotLox;

public class DotLoxInstance {
	private DotLoxClass klass;
	private readonly Dictionary<string, object> fields = new();

	public DotLoxInstance(DotLoxClass klass) {
		this.klass = klass;
	}

	public object Get(Token name) {
		// Search for field
		if (fields.ContainsKey(name.getLexeme())) {
			return fields[name.getLexeme()];
		}

		// Search for method, shadowed by field
		DotLoxFunction? method = klass.FindMethod(name.getLexeme());

		// Handle new scope
		if (method != null) return method.Bind(this);

		throw new RuntimeError(name, $"Undefined property '{name.getLexeme()}'.");
	}

	public void Set(Token name, object value) {
		fields[name.getLexeme()] = value;
	}

    public override string ToString() {
        return klass.getName() + " instance";
    }
}
