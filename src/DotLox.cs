using DotLox.Exception;

namespace DotLox;

public class DotLox {

	internal static bool hadError = false;
	internal static bool hadRuntimeError = false;

	private static readonly Interpreter interpreter = new Interpreter();

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
		if (hadRuntimeError) Environment.Exit(70);
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
		List<Stmt> statements = parser.Parse();

		if (hadError) return;

		interpreter.Interpret(statements);
	}

	internal static void HandleError(int line, string message) {
		Report(line, "", message);
	}

	internal static void HandleError(Token token, string message) {
		if (token.getType() == TokenType.EOF) {
			Report(token.getLine(), " at end", message);
		} else {
			Report(token.getLine(), $" at '{token.getLexeme()}'", message);
		}
	}

	internal static void HandleRuntimeError(RuntimeError e) {
		Console.Error.WriteLine($"{e.Message} \n[line {e.token.getLine()}]");
		hadRuntimeError = true;
	}

	private static void Report(int line, string where, string message) {
		Console.Error.WriteLine($"[line {line}] Error {where}: {message}");
		hadError = true;
	}
}
