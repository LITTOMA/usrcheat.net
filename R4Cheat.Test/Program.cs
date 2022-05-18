// See https://aka.ms/new-console-template for more information
using R4Cheat;

var cheat = new R4Cheat.R4Cheat(@"D:\GameDev\DS\usrcheat_3.dat");
System.Console.WriteLine(cheat.Header.Title);

await cheat.LoadAllGames(new Progress<ProgressArgs>(args =>
{
    System.Console.WriteLine($"{args.Current}/{args.Max}");
}));

System.Console.WriteLine("Done");