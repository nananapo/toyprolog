using toyprolog.AST;

namespace toyprolog;

public static class Assert
{
  public static bool HasVariableInDefinition(ITerm term)
  {
    return term.IsAmbiguous();
  }
  
  public static void AssertDefinitionSentence(ISentence sentence)
  {
    if (sentence is HeadSentence headSentence)
    {
      if (HasVariableInDefinition(headSentence.Head))
        throw new Exception("singleton variables");
    }
  }
}