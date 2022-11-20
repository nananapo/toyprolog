namespace toyprolog.Token;

public class SymbolToken : IToken
{
  public enum SymbolType{
    None,
    開き括弧,
    閉じ括弧,
    連言,
    選択,
    述語,
    カット,
    無視,
    文の終了
  }

  public readonly SymbolType Type;

  public SymbolToken(SymbolType type)
  {
    Type = type;
  }
  

  public override string ToString()
  {
    return Type.ToString();
  }
}