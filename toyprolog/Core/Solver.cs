using toyprolog.AST;

namespace toyprolog;

public class Solver
{
  private readonly List<ISentence> _globalSentences;
  private readonly HeadSentence _startQuery;

  private bool _isExecutedOnce = false;

  public bool IsContinue { get; private set; } = false;

  private readonly List<IRunEnv> _envStack = new ();

  public Solver(List<ISentence> sentences, HeadSentence query)
  {
    _globalSentences = sentences;
    _startQuery = query;
  }

  private string RunContinue()
  {
    while (true)
    {
      if (_envStack.Count == 0)
      {
        IsContinue = false;
        return "end exec";
      }

      var env = _envStack[^1];
      _envStack.Remove(env);

      if (env is GlobalEnv globalEnv)
      {
        if (globalEnv.GlobalIndex >= _globalSentences.Count)
        {
          IsContinue = false;
          return "end exec";
        }
        
        var sentence = _globalSentences[globalEnv.GlobalIndex];
        if (!sentence.Match(env.Query))
        {
          globalEnv.GlobalIndex++;
          _envStack.Add(globalEnv);
          continue;
        }

        return "match";
      }

      return "err";
    }
  }

  public string Run()
  {
    if (_isExecutedOnce && !IsContinue)
    {
      return "answer end";
    }

    if (!_isExecutedOnce)
    {
      _isExecutedOnce = true;
      _envStack.Add(new GlobalEnv(0, false, _startQuery));
    }

    var result = RunContinue();
    return result;
  }
}