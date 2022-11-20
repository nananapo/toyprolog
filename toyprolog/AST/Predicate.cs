namespace toyprolog.AST;

public class Predicate : IPredicate
{
  public ITerm Term;

  public Predicate(ITerm term)
  {
    Term = term;
  }
}