using System;
using System.IO;

using DotLox.Test;

namespace DotLox;

public class DotLox {

	internal static bool hadError = false;

	public static void Main(string[] args) {
		if (args.Length > 1) {
			Console.WriteLine("Usage: dotlox [script|test]");
			Environment.Exit(64);
		} else if (args.Length == 1 && !(args[0].Equals("test"))) {
			RunFile(args[0]);
		} else if (args.Length == 1 && args[0].Equals("test")) {
			RunTest();
		} else {
			RunPrompt();
		}
	}

	private static void RunTest() {
		Expr expression = new Expr.Binary(
				new Expr.Unary(
					new Token(TokenType.MINUS, "-", null, 1),
					new Expr.Literal(123)),
				new Token(TokenType.STAR, "*", null, 1),
				new Expr.Grouping(
					new Expr.Literal(45.67)));

		Console.WriteLine(new ASTPrinter().Print(expression));
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

		foreach (Token token in tokens) {
			Console.WriteLine(token);
		}
	}

	// TODO: Make an Exception class
	internal static void Error(int line, string message) {
		Report(line, "", message);
	}

	private static void Report(int line, string where, string message) {
		Console.Error.WriteLine($"[line {line}] Error {where}: {message}");
		hadError = true;
	}
}
