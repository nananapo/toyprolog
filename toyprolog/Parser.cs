using toyprolog.AST;
using toyprolog.Token;

#nullable disable
namespace toyprolog;

public static class Parser
{
  
  private static bool ConsumeSymbol(List<IToken> tokens, ref int index, SymbolToken.SymbolType type)
  {
    if (index >= tokens.Count) return false;
    if (tokens[index] is not SymbolToken symbolToken)
      return false;
    if (symbolToken.Type != type)
      return false;
    index++;
    return true;
  }
  
  private static bool ConsumeVariable(List<IToken> tokens, ref int index, out Variable variable)
  {
    variable = null;
    if (index >= tokens.Count) return false;
    if (tokens[index] is not StringToken stringToken) return false;
    if (Char.IsLower(stringToken.Value[0]))
      return false;
    index++;
    variable = new Variable(stringToken.Value);
    return true;
  }
  
  private static bool ConsumeAtom(List<IToken> tokens, ref int index, out Atom atom)
  {
    atom = null;
    if (index >= tokens.Count) return false;
    
    if (tokens[index] is StringToken stringToken)
    {
      if (Char.IsUpper(stringToken.Value[0]))
        return false;
      index++;
      atom = new Atom(stringToken.Value);
      return true;
    }

    if (tokens[index] is NumToken numToken)
    {
      atom = new Atom(numToken.Number.ToString());
      index++;
      return true;
    }

    return false;
  }

  public static ITerm ExpectTerm(List<IToken> tokens, ref int index)
  {
    if (ConsumeAtom(tokens, ref index, out Atom atom))
    {
      if (!ConsumeSymbol(tokens, ref index, SymbolToken.SymbolType.開き括弧))
        return atom;
      
      var terms = new List<ITerm>();
      while (true)
      {
        var term = ExpectTerm(tokens, ref index);
        terms.Add(term);

        if (ConsumeSymbol(tokens, ref index, SymbolToken.SymbolType.連言))
          continue;
        if (ConsumeSymbol(tokens, ref index, SymbolToken.SymbolType.閉じ括弧))
          break;
        throw new Exception("toji kakko not found");
      }
      
      return new MultTerm(atom, terms);
    }

    if (ConsumeVariable(tokens, ref index, out Variable variable))
    {
      return variable;
    }

    throw new Exception("failed to parse term");
  }

  public static IPredicate ExpectPredicate(List<IToken> tokens, ref int index)
  {
    if (ConsumeSymbol(tokens, ref index, SymbolToken.SymbolType.カット))
    {
      return new CutPredicate();
    }

    var term = ExpectTerm(tokens, ref index);
    return new Predicate(term);
  }

  public static IClause ExpectClause(List<IToken> tokens, ref int index)
  {
    var term = ExpectPredicate(tokens, ref index);
    
    if (ConsumeSymbol(tokens, ref index, SymbolToken.SymbolType.連言))
    {
      var term2 = ExpectClause(tokens, ref index);
      return new ConjunctionClause(term, term2);
    }

    if (ConsumeSymbol(tokens, ref index, SymbolToken.SymbolType.選択))
    {
      var term2 = ExpectClause(tokens, ref index);
      return new SelectClause(term, term2);
    }

    return new Clause(term);
  }

  private static ISentence ExpectSentence(List<IToken> tokens, ref int index)
  {
    var term = ExpectTerm(tokens, ref index);
    // TODO := 節
    if (term == null)
      throw new Exception("failed to parse");

    ISentence sentence;
    if (ConsumeSymbol(tokens, ref index, SymbolToken.SymbolType.述語))
    {
      var clause = ExpectClause(tokens, ref index);
      sentence = new EqualSentence(term, clause);
    }
    else
    {
      sentence = new HeadSentence(term);
    }

    if (!ConsumeSymbol(tokens, ref index, SymbolToken.SymbolType.文の終了))
      throw new Exception("bun ga syuuryou sinai yo~");
    
    return sentence;
  }
  
  public static List<ISentence> Run(List<IToken> tokens)
  {
    List<ISentence> sentences = new();

    int index = 0;
    while (index < tokens.Count)
    {
      sentences.Add(ExpectSentence(tokens, ref index));
    }

    return sentences;
  }
}