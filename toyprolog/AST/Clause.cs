namespace toyprolog.AST;

public class Clause : IClause
{
  public readonly IPredicate Predicate;

  public Clause(IPredicate predicate)
  {
    Predicate = predicate;
  }
}