namespace DotLox;

public class DotLoxClass {
	private readonly string name;

	public string getName() {
		return name;
	}

	public DotLoxClass(string name) {
		this.name = name;
	}

    public override string ToString()
    {
        return name;
    }
}
