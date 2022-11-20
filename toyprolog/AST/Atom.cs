namespace toyprolog.AST;

public class Atom : ITerm
{
  public readonly string Name;

  public Atom(string name)
  {
    Name = name;
  }

  public bool Match(ITerm term)
  {
    if (term is not Atom atom)
      return false;
    return atom.Name == Name;
  }
}