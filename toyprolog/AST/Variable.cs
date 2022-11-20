namespace toyprolog.AST;

public class Variable : ITerm
{
  public readonly string Name;

  public Variable(string name)
  {
    Name = name;
  }

  public bool Match(ITerm term)
  {
    return true;
  }
}