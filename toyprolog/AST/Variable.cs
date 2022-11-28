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

  public bool MatchPerfect(ITerm term)
  {
    return true;
  }

  public bool IsAmbiguous()
  {
    return true;
  }

  public Dictionary<Variable, List<ITerm>> ResolveVar(ITerm lt)
  {
    var dict = new Dictionary<Variable, List<ITerm>>();
    dict[this] = new List<ITerm>{lt};
    return dict;
  }

  public Dictionary<ITerm, List<ITerm>> ResolveTerm(ITerm lt)
  {
    var dict = new Dictionary<ITerm, List<ITerm>>();
    dict[this] = new List<ITerm>{lt};
    return dict;
  }

  public Dictionary<ITerm, List<ITerm>> ResolveRight(ITerm right)
  {
    var dict = new Dictionary<ITerm, List<ITerm>>();
    dict[this] = new List<ITerm>{right};
    return dict;
  }

  public ITerm Apply(Dictionary<ITerm, ITerm> dict)
  {
    var same = dict.Select(v=>v.Key).Where(v => v.Equals(this)).ToList();
    if (same.Count == 0)
    {
      return this;
    }
    return dict[same[0]];
  }

  protected bool Equals(Variable other)
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
    return Equals((Variable) obj);
  }

  public override int GetHashCode()
  {
    return ToString().GetHashCode();
  }

  public override string ToString()
  {
    return Name;
  }
}