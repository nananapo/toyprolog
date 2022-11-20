using toyprolog.AST;

namespace toyprolog;

public interface IRunEnv
{
  public HeadSentence Query { get; set; }
  public bool IsSatisfied { get; set; }
}