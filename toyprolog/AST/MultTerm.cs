namespace toyprolog.AST;

public class MultTerm : ITerm
{
  public readonly Atom Atom;

  public readonly List<ITerm> Terms;

  public MultTerm(Atom atom, List<ITerm> terms)
  {
    Atom = atom;
    Terms = terms;
  }

  public bool Match(ITerm term)
  {
    if (term is not MultTerm multTerm)
      return false;
    
    if (!Atom.Match(multTerm.Atom)
        || Terms.Count != multTerm.Terms.Count)
      return false;
    
    for (int i = 0; i < Terms.Count; i++)
    {
      if (!Terms[i].Match(multTerm.Terms[i]))
        return false;
    }
    return true;
  }
}