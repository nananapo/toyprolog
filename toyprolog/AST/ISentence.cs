namespace toyprolog.AST;

public interface ISentence
{
  bool Match(HeadSentence sentence);
}