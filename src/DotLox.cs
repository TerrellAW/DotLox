using DotLox.Test;

namespace DotLox;

public class DotLox {

	internal static bool hadError = false;

	public static void Main(string[] args) {
		if (args.Length > 1) {
			Console.WriteLine("Usage: dotlox [script|test]");
			Environment.Exit(64);
		} else if (args.Length == 1) {
			RunFile(args[0]);
		} else {
			RunPrompt();
		}
	}

	private static void RunFile(string path) {
		Run(File.ReadAllText(path));

		if (hadError) Environment.Exit(65);
	}

	private static void RunPrompt() {
		for (;;) {
			Console.Write("> ");
			string? line = Console.ReadLine();
			if (line == null) break;
			Run(line);
			hadError = false;
		}
	}

	private static void Run(string source) {
		Scanner scanner    = new Scanner(source);
		List<Token> tokens = scanner.ScanTokens();

		Parser parser = new Parser(tokens);
		Expr expr = parser.Parse();

		if (hadError) return;

		Console.WriteLine(new ASTPrinter().Print(expr));
	}

	internal static void Error(int line, string message) {
		Report(line, "", message);
	}

	internal static void Error(Token token, string message) {
		if (token.getType() == TokenType.EOF) {
			Report(token.getLine(), " at end", message);
		} else {
			Report(token.getLine(), $" at '{token.getLexeme()}'", message);
		}
	}

	private static void Report(int line, string where, string message) {
		Console.Error.WriteLine($"[line {line}] Error {where}: {message}");
		hadError = true;
	}
}
