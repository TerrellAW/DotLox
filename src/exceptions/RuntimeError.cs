namespace DotLox.Exception;

public class RuntimeError : System.Exception {
	readonly Token? token;

	public RuntimeError(Token? token, string message) : base(message) {
		if (token != null) {
			this.token = token;
		}
	}
}
