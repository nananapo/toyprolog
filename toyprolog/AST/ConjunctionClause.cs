namespace toyprolog.AST;

public class ConjunctionClause : IClause
{
  public readonly IPredicate Left;
  public readonly IClause Right;

  public ConjunctionClause(IPredicate left, IClause right)
  {
    Left = left;
    Right = right;
  }
}