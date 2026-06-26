namespace DotLox;

public interface DotLoxCallable {
	public int Arity();
	public Object? Call(Interpreter interpreter, List<Object> arguments);
}
