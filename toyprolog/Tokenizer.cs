using toyprolog.Token;

namespace toyprolog;

public static class Tokenizer
{

  private static readonly  List<(string, SymbolToken.SymbolType)> SymbolDict = new()
  {
    ("(", SymbolToken.SymbolType.開き括弧),
    (")", SymbolToken.SymbolType.閉じ括弧),
    ("!", SymbolToken.SymbolType.カット),
    (";", SymbolToken.SymbolType.選択),
    (",", SymbolToken.SymbolType.連言),
    (".", SymbolToken.SymbolType.文の終了),
    (":-", SymbolToken.SymbolType.述語)
  };

  private static (int result, int index) ReadNumber(string program, int index)
  {
    string str = "";
    for (; index < program.Length; index++)
    {
      var c = program[index];
      if (!Char.IsDigit(c))
        break;
      str += c;
    }

    if (int.TryParse(str, out int result))
      return (result, index);
    
    throw new Exception("failed to parse num");
  }

  public static (bool result, int index, SymbolToken.SymbolType) ReadSymbol(string program, int index)
  {
    foreach (var (str, symbol) in SymbolDict)
    {
      if (program.StartsWith(index, str))
      {
        return (true, index + str.Length, symbol);
      }
    }
    return (false, index, SymbolToken.SymbolType.None);
  }

  public static (bool result, int index, string str) ReadString(string program, int index)
  {
    var result = "";
    for (; index < program.Length; index++)
    {
      // 数字は読んでいるはずなので考えない
      var c = program[index];
      if (('A' <= c && c <= 'Z') 
          || ('a' <= c && c <= 'z') 
          || Char.IsDigit(c) 
          || c == '_')
      {
        result += c;
        continue;
      }
      break;
    }
    return (result.Length > 0, index, result);
  }

  public static (bool result, int index, string str) ReadStrLiteral(string program, int index)
  {
    var result = "";
    var isClosed = false;
    var startIndex = index;

    // 範囲外
    if (index >= program.Length || program[index] != '\"')
    {
      return (false, index, result);
    }

    index += 1;

    for (; index < program.Length; index++)
    {
      if (program[index] == '"')
      {
        isClosed = true;
        break;
      }
      if (program[index] == '\\')
      {
        index += 1;
        if (index >= program.Length)
        {
          break;
        }
        switch(program[index])
        {
          case 'n':
            result += "\n";
             break;
          default:
            throw new Exception("invalid escape sequence");
        }
        continue;
      }
      result += program[index];
    }

    if (!isClosed)
    {
       return (false, startIndex, "");
    }
    return (true, index + 1, result);
  }

  public static List<IToken> Run(string program)
  {
    var result = new List<IToken>();
    
    int index = 0;
    while (index < program.Length)
    {
      var c = program[index]; 
      //Console.WriteLine(c);
      
      if (Char.IsWhiteSpace(c))
      {
        index++;
        continue;
      }
      if (Char.IsDigit(c))
      {
        (int number, index) = ReadNumber(program, index);
        result.Add(new NumToken(number));
        continue;
      }

      (bool success, index, var symbolType) = ReadSymbol(program, index);
      if (success)
      {
        result.Add(new SymbolToken(symbolType));
        continue;
      }

      (success, index, string str) = ReadString(program, index);
      if (success)
      {
        result.Add(new StringToken(str));
        continue;
      }

      (success, index, str) = ReadStrLiteral(program, index);
      if (success)
      {
        result.Add(new StringToken(str));
        continue;
      }
      
      throw new Exception("failed to tokenize :" + index + ", " + (int)c);
    }
    return result;
  }
}
