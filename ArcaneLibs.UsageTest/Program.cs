// See https://aka.ms/new-console-template for more information

using System.Drawing;
using ArcaneLibs;
using ArcaneLibs.Collections;
using ArcaneLibs.Extensions;
using ArcaneLibs.Logging;
using ArcaneLibs.Logging.LogEndpoints;
using ArcaneLibs.UsageTest;

Console.WriteLine("Hello, World!");
//create logs
var log1 = new LogManager();
var log2 = new LogManager();
var log3 = new LogManager();
//add endpoints
log1.AddEndpoint(new ConsoleEndpoint());
log2.AddEndpoint(new DebugEndpoint());
log3.AddEndpoint(new FileEndpoint("out.txt", false));
//test logs
log1.Log("console log");
log1.LogDebug("console dbg log");
log2.Log("debug log");
log2.LogDebug("debug dbg log");
log3.Log("file log");
log3.LogDebug("file dbg log");

/*
Console.Write("Expected: ");
for (int i = 0; i < 5; i++)
{
    Console.Write($"{MathUtil.Map(i, 0, 5, 0, 25)} ");
}
Console.Write("\nReversed: ");
for (int i = 0; i < 5; i++)
{
    Console.Write($"{MathUtil.Map(i, 0, 5, 25, 0)} ");
}
*/
var autodict = new AutoPopulatingDictionary<int, string>();
var autodict3 = new AutoPopulatingDictionary<int, Point>();

var a = autodict[3];
var c = autodict3[6];

//return;

Console.WriteLine(Util.GetCommandOutputSync("bash", "-c asdf"));
Console.WriteLine(Util.GetCommandOutputSync("bash", "-c ls"));

RandomClass arc = new RandomClass() {
    Id = 1,
    Name = "asdf",
    DateCreated = DateTime.Now,
    SubClassInst = new() {
        Description = "asdf"
    }
};
RandomClass brc = new RandomClass() {
    Id = 2,
    Name = "asdf",
    DateCreated = DateTime.Now,
    SubClassInst = new() {
        Description = "asdf"
    }
};

var differences = arc.FindDifferencesDeep(brc);
Console.WriteLine(differences.left.ToJson());
Console.WriteLine(differences.right.ToJson());
