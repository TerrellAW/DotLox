namespace DotLox.Exception;

public class ParseError : System.Exception {

	internal ParseError(Token token, string message) {
		DotLox.Error(token, message);
	}
}
