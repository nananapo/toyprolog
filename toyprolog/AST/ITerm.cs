namespace toyprolog.AST;

public interface ITerm
{
  /// <summary>
  /// 外形が一致しているかを確認する
  /// </summary>
  /// <param name="term"></param>
  /// <returns></returns>
  bool Match(ITerm term);

  /// <summary>
  /// 外形が完全に一致する
  /// つまり、suc(T)はTに一致するが、Tはsuc(T)に一致しない
  /// </summary>
  /// <param name="term"></param>
  /// <returns></returns>
  bool MatchPerfect(ITerm term);

  bool IsAmbiguous();
  
  Dictionary<Variable, List<ITerm>> ResolveVar(ITerm lt);
  
  Dictionary<ITerm, List<ITerm>> ResolveTerm(ITerm lt);

  Dictionary<ITerm, List<ITerm>> ResolveRight(ITerm right);

  ITerm Apply(Dictionary<ITerm, ITerm> dict);

  bool Equals(ITerm term);
}