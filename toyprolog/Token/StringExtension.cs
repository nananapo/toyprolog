namespace toyprolog.Token;

public static class StringExtension
{
  public static bool StartsWith(this string source, int startIndex, string str)
  {
    for (int i = 0; i + startIndex < source.Length && i < str.Length; i++)
    {
      if (source[i + startIndex] != str[i]) return false;
    }
    return true;
  }
}