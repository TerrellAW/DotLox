using DotLox.Exception;

namespace DotLox;

public class DotLoxInstance {
	private DotLoxClass klass;
	private readonly Dictionary<string, object> fields = new();

	public DotLoxInstance(DotLoxClass klass) {
		this.klass = klass;
	}

	public object Get(Token name) {
		if (fields.ContainsKey(name.getLexeme())) {
			return fields[name.getLexeme()];
		}
		throw new RuntimeError(name, $"Undefined property '{name.getLexeme()}'.");
	}

	public void Set(Token name, object value) {
		fields[name.getLexeme()] = value;
	}

    public override string ToString() {
        return klass.getName() + " instance";
    }
}
