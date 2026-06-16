using DotLox.Exception;

namespace DotLox;

public class DotLoxEnv {
	private readonly Dictionary<string, object> values = new Dictionary<string, object>();

	public object Get(Token name) {
		if (values.ContainsKey(name.getLexeme())) {
			return values[name.getLexeme()];
		}

		throw new RuntimeError(name, $"Undefined variable '{name.getLexeme()}'.");
	}

	public void Define(string name, object? value) {
		values.Add(name, value);
	}
}
