// See https://aka.ms/new-console-template for more information
using R4Cheat;

var cheat = new R4Cheat.R4Cheat(args[0], decoding: R4Cheat.Misc.TryGetEncoding("GBK"));
System.Console.WriteLine(cheat.Header.Title);

var progress = new Progress<ProgressArgs>(args =>
    {
        System.Console.WriteLine($"{args.Current}/{args.Max} {args.Message}");
    }
);

Task.Run(() =>
{
    cheat.LoadAllGames(progress);
    System.Console.WriteLine("Done");
}).Wait();