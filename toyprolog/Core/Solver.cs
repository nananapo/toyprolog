using toyprolog.AST;

namespace toyprolog;

// TODO _
public class Solver
{
  private readonly List<ISentence> _globalSentences;
  private readonly HeadSentence _startQuery;

  private bool _isAnsweredOnce = false;

  private readonly List<IRunEnv> _envStack = new ();
  
  public Solver(List<ISentence> sentences, HeadSentence query)
  {
    _globalSentences = sentences;
    _startQuery = query;
  }

  /// <summary>
  /// 例外で再帰を抜ける非常に邪悪なコード
  /// </summary>
  /// <param name="answer"></param>
  /// <param name="isContinue"></param>
  /// <exception cref="Exception"></exception>
  private void PrintAnswer(string answer, bool isContinue = true)
  {
    _isAnsweredOnce = true;
    
    Console.Write(answer);
    Console.Write(" ");
    
    if (!isContinue || Console.ReadKey().KeyChar != ';')
    {
      Console.WriteLine();
      Exit();
    }
    Console.WriteLine();
  }

  private void PrintAnswer(Dictionary<Variable, ITerm> terms, bool isContinue = true)
  {
    string answer = "";
    int index = 0;
    foreach (var (name,term) in terms)
    {
      answer += name + " = " + term;
      index++;
      if (index != terms.Count)
      {
        answer += "\n";
      }
    }
    PrintAnswer(answer, isContinue);
  }

  private void Exit()
  {
    throw new SolveEndException();
  }

  private bool ConsumeCommands(ITerm term)
  {
    var writeCommand = new MultTerm(new Atom("write"), new List<ITerm>{new Variable("T")});
	var nlCommand = new Atom("nl");

    // write
    if (term.Match(writeCommand))
    {
      var resolve = writeCommand.ResolveTerm(term);
      foreach (var (k,vl) in resolve)
      {
        if (vl.Count != 1)
        {
          continue;
        }
        if (vl[0] is not Atom)
        {
          // TODO ここエラー
          continue;
        }
        Console.WriteLine(vl[0].ToString());
        return true;
      }
    }
    // nl
    if (nlCommand.Equals(term))
    {
      Console.WriteLine();
      return true;
    }
    return false;
  }

  private void SolveGlobal(HeadSentence query, Action<Dictionary<Variable, ITerm>>? callback)
  {

    // クエリがコマンドなら終了
    if (ConsumeCommands(query.Head))
    {
      if (callback != null)
      {
        callback(new Dictionary<Variable, ITerm>());
      }
      return;
    }

    foreach (var sentence in _globalSentences)
    {
      // 外形が一致しないなら終了
      if (!sentence.Match(query))
        continue;

      if (sentence is HeadSentence headSentence)
      {
        var head = headSentence.Head;
        if (head is Atom)
        {
          if (callback == null)
          {
            PrintAnswer("true", false);
          }
          else
          {
            callback(new Dictionary<Variable, ITerm>());
          }
        }
        else if (head is MultTerm mult)
        {
          var matchAll = true;
          var def = new Dictionary<Variable, ITerm>();
          var qMult = (MultTerm) query.Head;

          // 変数に値を割り当てる
          for (int i = 0; i < mult.Terms.Count; i++)
          {
            var lt = mult.Terms[i];
            var rt = qMult.Terms[i];

            // 曖昧ではない
            if (!rt.IsAmbiguous())
            {
              continue;
            }

            foreach (var (name, term) in rt.ResolveVar(lt))
            {
              if (term.Count > 2)
              {
                matchAll = false;
                break;
              }

              if (!def.ContainsKey(name))
              {
                def[name] = term[0];
                continue;
              }

              if (!def[name].Equals(term[0]))
              {
                matchAll = false;
                break;
              }
            }

            // 不一致
            if (!matchAll) break;
          }

          if (!matchAll)
          {
            continue;
          }

          if (callback == null)
          {
            if (def.Count == 0)
            {
              PrintAnswer("true", false);
            }
            else
            {
              PrintAnswer(def);
            }
          }
          else
          {
            callback(def);
          }
        }
        else
        {
          // TODO そもそもHeadにVariableは来ないはずなのでエラーを出す
          throw new NotImplementedException();
        }
      }
      else if (sentence is EqualSentence equalSentence)
      {
        if (equalSentence.Clause is Clause clause)
        {
          // TODO predicateがpredicateでない場合はエラー
          if (clause.Predicate is not Predicate predicate)
          {
            throw new NotImplementedException();
          }

          if (equalSentence.Head is Atom atom)
          {
            // Atomは必ず一致して、かつ曖昧性がない
            SolveGlobal(new HeadSentence(atom), callback);
          }
          else if (equalSentence.Head is MultTerm mult)
          {
            // とりあえず全探索している
            var newCallback = (Dictionary<Variable, ITerm> dict) =>
            {
              // 曖昧性がない
              if (!query.Head.IsAmbiguous())
              {
                // 他にコールバックがない => true
                if (callback == null)
                {
                  PrintAnswer("true", false);
                }
                else
                {
                  callback(new Dictionary<Variable, ITerm>());
                }
              }
              else
              {
                var solvedTerm = mult.Apply(dict.ToDictionary(v=>(ITerm)v.Key, v=>v.Value));
                var solvedQuery = query.Head.ResolveRight(solvedTerm);

                /*
                Console.WriteLine("callback");
                foreach (var (d,v) in dict)
                {
                  Console.WriteLine(d + " => " + v);
                }
                Console.WriteLine(mult + " -> " + solvedTerm + " -> ");
                foreach (var (d,v) in solvedQuery)
                {
                  Console.WriteLine(d + " rs=> " + string.Join(" ", v));
                }
                */

                // 整合性のチェック
                var resultList = new Dictionary<Variable, ITerm>();
                foreach (var (q,v) in solvedQuery)
                {
                  if (v.Count != 1)
                    return;
                  if (q is not Variable variable)
                    return;
                  resultList[variable] = v[0];
                }
                
                if (callback == null)
                {
                  PrintAnswer(resultList);
                }
                else
                {
                  callback(resultList);
                }
              }
            };
            SolveGlobal(new HeadSentence(predicate.Term), newCallback);
/*
            var solvedDict = new Dictionary<ITerm, ITerm>();
            
            
            
            var updated = true;
            var isValid = true;
            while (updated && isValid)
            {
              updated = false;

              mult = (MultTerm)mult.Apply(solvedDict);
              var dict = qMult.ResolveRight(mult);

              foreach (var (left, children) in dict)
              {
                // TODO 解決済みのものを適用 ?

                // クエリは曖昧ではない => 右辺が曖昧
                if (!qMult.IsAmbiguous())
                {
                  // TODO 右辺にクエリをあてはめて、右辺の変数を解決する
                  // 複数の答えが出たらその時点で失敗
                  updated = true;
                  continue;
                }

                // 右辺は曖昧ではなく、クエリは曖昧
                if (qMult.IsAmbiguous() && !mult.IsAmbiguous())
                {
                  // TODO
                  // クエリに右辺をあてはめて変数を解決する
                  // 複数の答えが出たらその時点で失敗
                  updated = true;
                  continue;
                }
                
                // 両辺とも曖昧
                // TODO そのまま？
              }
            }

            if (!isValid)
              continue;
*/
          }
          else if (equalSentence.Head is Variable)
          {
            // TODO 文法的なエラー
            throw new NotImplementedException();
          }
        }
        else
        {
          throw new NotImplementedException();
        }
        // Varは両方相容れないならダメ
        // 片方か両方なら狭いほうを採用
      }
    }
  }

  public void Run()
  {
    try
    {
      SolveGlobal(_startQuery, null);
    }
    catch (SolveEndException)
    {
    }

    if (!_isAnsweredOnce)
    {
      Console.WriteLine("false");
    }
  }
}
