namespace toyprolog.AST;

public class EqualSentence : ISentence
{
  public readonly ITerm Head;
  public readonly IClause Clause;

  public EqualSentence(ITerm term, IClause clause)
  {
    Head = term;
    Clause = clause;
  }

  public bool Match(HeadSentence sentence)
  {
    return false;
    throw new NotImplementedException();
  }
}