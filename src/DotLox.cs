using DotLox.Exception;

namespace DotLox;

// Main program class
public class DotLox {

	// Error tracking
	internal static bool hadError = false;
	internal static bool hadRuntimeError = false;

	// Initialize static interpreter
	private static readonly Interpreter interpreter = new Interpreter();

	// Handle user arguments and run file or REPL
	public static void Main(string[] args) {
		// Only one arg is valid, gives usage instructions
		if (args.Length > 1) {
			Console.WriteLine("Usage: dotlox [script|test]");
			Environment.Exit(64);
		// Considers arg to be file path
		} else if (args.Length == 1) {
			RunFile(args[0]);
		// If no arg, run REPL
		} else {
			RunPrompt();
		}
	}

	// Read file and execute it or fail with exit code
	private static void RunFile(string path) {
		Run(File.ReadAllText(path));

		if (hadError) Environment.Exit(65);
		if (hadRuntimeError) Environment.Exit(70);
	}

	// Keep REPL running and process user input
	private static void RunPrompt() {
		// Loop forever
		for (;;) {
			// Prompt
			Console.Write("> ");

			// Read user input
			string? line = Console.ReadLine();
			// Ends program when user enters Ctrl + C
			if (line == null) break;
			// Processes input
			Run(line);
			// Continues loop even if error occurred
			hadError = false;
		}
	}

	// Runs the interpreter
	private static void Run(string source) {
		// Scan input for tokens
		Scanner scanner    = new Scanner(source);
		List<Token> tokens = scanner.ScanTokens();

		// Parse tokens into valid statements and expressions
		Parser parser = new Parser(tokens);
		List<Stmt> statements = parser.Parse();

		// Do not execute invalid code
		if (hadError) return;

		// Execute code
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
