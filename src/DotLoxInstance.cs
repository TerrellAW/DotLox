namespace DotLox;

public class DotLoxInstance {
	private DotLoxClass klass;

	public DotLoxInstance(DotLoxClass klass) {
		this.klass = klass;
	}

    public override string ToString() {
        return klass.getName() + " instance";
    }
}
