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
    if (term is Variable)
      return true;
    return MatchPerfect(term);
  }

  public bool MatchPerfect(ITerm term)
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

  public bool IsAmbiguous()
  {
    for (int i = 0; i < Terms.Count; i++)
    {
      if (Terms[i].IsAmbiguous())
        return true;
    }
    return false;
  }

  public Dictionary<Variable, List<ITerm>> ResolveVar(ITerm lt)
  {
    var dict = new Dictionary<Variable, List<ITerm>>();
    if (!MatchPerfect(lt))
    {
      return dict;
    }

    var mult = (MultTerm)lt;
    for (int i = 0; i < Terms.Count; i++)
    {
      foreach (var (name, term) in Terms[i].ResolveVar(mult.Terms[i]))
      {
        if (!dict.ContainsKey(name))
        {
          dict[name] = new List<ITerm>();
        }
        dict[name].AddRange(term.Where(v=>!dict[name].Contains(v)));
      }
    }
    return dict;
  }

  public Dictionary<ITerm, List<ITerm>> ResolveTerm(ITerm lt)
  {
    var dict = new Dictionary<ITerm, List<ITerm>>();
    if (lt is not MultTerm mult)
    {
      return dict;
    }
    
    for (int i = 0; i < Terms.Count; i++)
    {
      foreach (var (name, term) in Terms[i].ResolveTerm(mult.Terms[i]))
      {
        if (!dict.ContainsKey(name))
        {
          dict[name] = new List<ITerm>();
        }
        dict[name].AddRange(term.Where(v=>!dict[name].Contains(v)));
      }
    }
    return dict;
  }

  public Dictionary<ITerm, List<ITerm>> ResolveRight(ITerm right)
  {
    var dict = new Dictionary<ITerm, List<ITerm>>();
    
    if (Match(right))
    {
      if (right is Variable variable)
      {
        dict[this] = new List<ITerm> { variable };
      }
      else if (right is MultTerm multTerm)
      {
        for (int i = 0; i < Terms.Count; i++)
        {
          foreach (var (left, childlen) in Terms[i].ResolveRight(multTerm.Terms[i]))
          {
            if (!dict.ContainsKey(left))
            {
              dict[left] = new List<ITerm>();
            }
            foreach (var child in childlen)
            {
              if (!dict[left].Contains(child))
              {
                dict[left].Add(child);
              }
            }
          }
        }
      }
      else
      {
        throw new InvalidOperationException("matchしないはず");
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
    if (!IsAmbiguous())
      return this;
    
    var same = dict.Select(v=>v.Key).Where(v => v.Equals(this)).ToList();
    
    // そのまま一致
    if (same.Count != 0)
    {
      return dict[same[0]];
    }

    // 中身が一致
    var ts = new List<ITerm>();
    for(int i = 0; i < Terms.Count; i++)
    {
      ts.Add(Terms[i].Apply(dict));
    }
    return new MultTerm(Atom, ts);
  }

  protected bool Equals(MultTerm other)
  {
    if (!Atom.Equals(other.Atom)
        || Terms.Count != other.Terms.Count)
      return false;

    for (int i = 0; i < Terms.Count; i++)
    {
      if (!other.Terms[i].Equals(Terms[i]))
        return false;
    }

    return true;
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
    return Equals((MultTerm) obj);
  }

  public override int GetHashCode()
  {
    return ToString().GetHashCode();
  }

  public override string ToString()
  {
    string str = Atom + "(";
    for (int i = 0; i < Terms.Count; i++)
    {
      str += Terms[i];
    }
    str += ")";
    return str;
  }
}