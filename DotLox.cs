using System;
using System.IO;

namespace DotLox;

public class DotLox {

	internal static bool hadError = false;

	public static void Main(string[] args) {
		if (args.Length > 1) {
			Console.WriteLine("Usage: dotlox [script]");
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
		// TODO: Make Scanner class
		//Scanner scanner    = new Scanner(source);
		//List<Token> tokens = scanner.scanTokens();

		//foreach (Token token in tokens) {
			//Console.WriteLine(token);
		//}

		// Placeholder
		Console.WriteLine(source);
	}

	// TODO: Make an Exception class
	internal static void Error(int line, string message) {
		Report(line, "", message);
	}

	private static void Report(int line, string where, string message) {
		Console.Error.WriteLine($"[line {line}] Error {where}: message");
		hadError = true;
	}
}
