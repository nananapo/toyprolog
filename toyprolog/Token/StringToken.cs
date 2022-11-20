namespace toyprolog.Token;

public class StringToken : IToken
{
  public readonly string Value;

  public StringToken(string str)
  {
    this.Value = str;
  }

  public override string ToString()
  {
    return Value;
  }
}