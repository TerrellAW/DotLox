/**
 * Source - https://github.com/Nrosa01/CSLox/blob/master/Runtime/Src/Lox/NativeFunction.cs
 * Programmed by Nrosa01, modified by me.
 * Retrieved 2026-06-20
 */

namespace DotLox;

internal class NativeFunction : DotLoxCallable {
	private int arity;
	private readonly Func<object, object, double> func;

	public NativeFunction(int arity, Func<object, object, double> func)
	{
		this.arity = arity;
		this.func = func;
	}

	public int Arity() {
		return arity;
	}

	public object? Call(Interpreter interpreter, List<object?> arguments) => func.Invoke(interpreter, arguments);

	public override string ToString() => "<native fn>";
}
