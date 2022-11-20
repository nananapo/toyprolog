namespace toyprolog.AST;

public interface ITerm
{
  /// <summary>
  /// 外形が一致しているかを確認する
  /// </summary>
  /// <param name="term"></param>
  /// <returns></returns>
  bool Match(ITerm term);
}