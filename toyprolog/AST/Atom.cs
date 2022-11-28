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
    if (term is Variable)
      return true;
    return MatchPerfect(term);
  }

  public bool MatchPerfect(ITerm term)
  {
    if (term is not Atom atom)
      return false;
    return atom.Name == Name;
  }

  public bool IsAmbiguous()
  {
    return false;
  }

  public Dictionary<Variable, List<ITerm>> ResolveVar(ITerm lt)
  {
    var dict = new Dictionary<Variable, List<ITerm>>();
    return dict;
  }

  public Dictionary<ITerm, List<ITerm>> ResolveTerm(ITerm lt)
  {
    var dict = new Dictionary<ITerm, List<ITerm>>();
    return dict;
  }

  public Dictionary<ITerm, List<ITerm>> ResolveRight(ITerm right)
  {
    var dict = new Dictionary<ITerm, List<ITerm>>();
    if (Match(right))
    {
      if (right.IsAmbiguous())
      {
        dict[this] = new List<ITerm>{right};
      }
    }   
    else
    {
      throw new InvalidOperationException("やめて～");
    }
    return dict;
  }

  public ITerm Apply(Dictionary<ITerm, ITerm> dict)
  {
    return this;
  }

  protected bool Equals(Atom other)
  {
    return Name == other.Name;
  }

  public bool Equals(ITerm? other)
  {
    return Equals((object?)other);
  }

  public override bool Equals(object? obj)
  {
    if (ReferenceEquals(null, obj)) return false;
    if (ReferenceEquals(this, obj)) return true;
    if (obj.GetType() != this.GetType()) return false;
    return Equals((Atom) obj);
  }

  public override int GetHashCode()
  {
    return Name.GetHashCode();
  }

  public override string ToString()
  {
    return Name;
  }
}