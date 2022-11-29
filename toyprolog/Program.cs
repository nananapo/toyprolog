using toyprolog;
using toyprolog.AST;

Console.WriteLine("toyprolog");

// load file
var global = new List<ISentence>();

void LoadFile(string fileName)
{
  // Console.WriteLine("Loading file...");
  try
  {
    var program = File.ReadAllText(fileName);
    var tokens = Tokenizer.Run(program);
    var sentences = Parser.Run(tokens);

    // assert
    foreach (var sentence in sentences)
    {
      Assert.AssertDefinitionSentence(sentence);
    }

    global.AddRange(sentences);
  }
  catch(Exception ex)
  {
    Console.WriteLine(ex);
    Console.WriteLine("failed to load file");
    return ;
  }
  // loaded
  Console.WriteLine("true");
}

bool RunCommand(ITerm term)
{

  if (term.IsAmbiguous())
  {
    return false;
  }

  var haltCommand = new Atom("halt");
  var consultCommand = new MultTerm(new Atom("consult"), new List<ITerm>{new Variable("T")});

  // halt
  if (term.Equals(haltCommand))
  {
    Console.WriteLine("Bye");
    System.Environment.Exit(0);
    return true;
  }

  // consult
  if (term.Match(consultCommand))
  {
    var resolve = consultCommand.ResolveTerm(term);
    foreach (var (k,vl) in resolve)
    {
      if (vl.Count != 1)
      {
        continue;
      }
      if (vl[0] is not Atom)
      {
        continue;
      }
      LoadFile(vl[0].ToString() + ".pl");
      return true;
    }
  }

  return false;
}

LoadFile("test/sample.pl");

while (true)
{
  Console.Write("?- ");

  var qStr = "";
  while (true)
  {
    qStr += Console.ReadLine();
    if (qStr.Contains('.'))
      break;
  }
  
  var tokens = Tokenizer.Run(qStr);
  var sentences = Parser.Run(tokens);

  if (sentences.Count == 0)
    continue;
  if (sentences.Count != 1)
  {
    Console.WriteLine("複数の質問はダメ");
    continue;
  }

  if (sentences[0] is not HeadSentence headSentence)
  {
    Console.WriteLine("質問は頭語のみ");
    continue;
  }

  if (headSentence.Head is Variable)
  {
    Console.WriteLine(">> 42 <<");
    continue;
  }

  var head = headSentence.Head;

  if (RunCommand(head))
  {
    continue ;
  }

  var solver = new Solver(global, headSentence);
  solver.Run();
}
