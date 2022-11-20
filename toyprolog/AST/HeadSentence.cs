namespace toyprolog.AST;

public class HeadSentence : ISentence
{
  public readonly ITerm Head;

  public HeadSentence(ITerm head)
  {
    Head = head;
  }

  public bool Match(HeadSentence target)
  {
    return Head.Match(target.Head);
  }
}