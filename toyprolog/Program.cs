using toyprolog;
using toyprolog.AST;

Console.WriteLine("toyprolog");

// load file
List<ISentence> global;
{
  Console.WriteLine("Loading file...");
  var program = File.ReadAllText("test/sample.pl");
  var tokens = Tokenizer.Run(program);
  var sentences = Parser.Run(tokens);
  global = sentences;
}

// assert
foreach (var sentence in global)
{
  Assert.AssertDefinitionSentence(sentence);
}

// loaded
Console.WriteLine("Loaded file : " + global.Count + " sentences");

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

  if (headSentence.Head.Equals(new Atom("halt")))
  {
    Console.WriteLine("終了");
    return;
  }

  var solver = new Solver(global, headSentence);
  solver.Run();
}