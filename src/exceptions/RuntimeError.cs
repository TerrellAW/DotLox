namespace DotLox.Exception;

public class RuntimeError : System.Exception {
	// TODO: Make private and use getter with null handling
	public readonly Token? token;

	public RuntimeError(Token? token, string message) : base(message) {
		if (token != null) {
			this.token = token;
		}
	}
}
