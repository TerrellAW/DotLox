/**
 * Source - https://github.com/Nrosa01/CSLox/blob/master/Runtime/Src/Lox/NativeFunction.cs
 * Programmed by Nrosa01, modified by me.
 * Retrieved 2026-06-20
 */

namespace DotLox;

internal class NativeFunction : DotLoxCallable {
	private int arity;
	private readonly Func<object, object, double> funcnum;
	private readonly Func<object, object, string?> funcstr;
	private readonly Func<object, object, bool> funcbool;

	public NativeFunction(int arity, Func<object, object, double> func)
	{
		this.arity = arity;
		this.funcnum = func;
	}

	public NativeFunction(int arity, Func<object, object, string?> func)
	{
		this.arity = arity;
		this.funcstr = func;
	}

	public NativeFunction(int arity, Func<object, object, bool> func)
	{
		this.arity = arity;
		this.funcbool = func;
	}

	public int Arity() {
		return arity;
	}

	public object? Call(Interpreter interpreter, List<object?> arguments) => 
		funcbool != null ? funcbool.Invoke(interpreter, arguments) : HandleNum(interpreter, arguments);

	private object? HandleNum(Interpreter interpreter, List<object?> arguments) {
		return funcnum != null ? funcnum.Invoke(interpreter, arguments) : funcstr.Invoke(interpreter, arguments);
	}

	public override string ToString() => "<native fn>";
}
