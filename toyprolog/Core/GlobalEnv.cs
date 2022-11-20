using toyprolog.AST;

namespace toyprolog;

public class GlobalEnv : IRunEnv
{
  public int GlobalIndex;

  public HeadSentence Query { get; set; }
  public bool IsSatisfied { get; set; }

  public GlobalEnv(int globalIndex, bool isSatisfied, HeadSentence query)
  {
    this.GlobalIndex = globalIndex;
    this.IsSatisfied = isSatisfied;
    this.Query = query;
  }
}