namespace toyprolog.Token;

public class NumToken : IToken
{
  public readonly int Number;
  public NumToken(int number)
  {
    Number = number;
  }

  public override string ToString()
  {
    return Number.ToString();
  }
}