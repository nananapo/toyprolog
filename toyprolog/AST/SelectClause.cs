namespace toyprolog.AST;

public class SelectClause : IClause
{
  public readonly IPredicate Left;
  public readonly IClause Right;

  public SelectClause(IPredicate left, IClause right)
  {
    Left = left;
    Right = right;
  }
}