using toyprolog;
using toyprolog.AST;

Console.WriteLine("toyprolog");
Console.WriteLine("Loading file...");

List<ISentence> global;
{
  var program = File.ReadAllText("test/sample.pl");
  var tokens = Tokenizer.Run(program);
  var sentences = Parser.Run(tokens);
  Console.WriteLine("Loaded file : " + sentences.Count + " sentences");
  /*
  foreach (var sentence in sentences)
    Console.WriteLine(sentence);
    */
  global = sentences;
}


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

  if (headSentence.Match(new HeadSentence(new Atom("halt"))))
  {
    Console.WriteLine("終了");
    return;
  }

  var solver = new Solver(global, headSentence);

  while (true)
  {
    var result = solver.Run();
    Console.WriteLine(result);
    
    if (!solver.IsContinue)
      break;
    
    Console.Write(" ");
    var q = Console.Read();
    if (q != ';')
      break;
  }
}