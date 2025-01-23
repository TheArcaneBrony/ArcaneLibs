using System.Drawing;
using ArcaneLibs;
using ArcaneLibs.Collections;
using ArcaneLibs.Logging;
using ArcaneLibs.Logging.LogEndpoints;
// ReSharper disable CollectionNeverUpdated.Local
// ReSharper disable UnusedVariable

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

// var arc = new RandomClass {
//     Id = 1,
//     Name = "asdf",
//     DateCreated = DateTime.Now,
//     SubClassInst = new RandomClass.SubClass {
//         Description = "asdf"
//     }
// };
// var brc = new RandomClass {
//     Id = 2,
//     Name = "asdf",
//     DateCreated = DateTime.Now,
//     SubClassInst = new RandomClass.SubClass {
//         Description = "asdf"
//     }
// };
//
// var differences = arc.FindDifferencesDeep(brc);
// Console.WriteLine(differences.left.ToJson());
// Console.WriteLine(differences.right.ToJson());

// identicon test
Directory.CreateDirectory("identicons");

// for (int i = 0; i < 100; i++) {
//     var ident = new SvgIdenticonGenerator();
//     var sw = Stopwatch.StartNew();
//     var count = 0;
//     while (sw.ElapsedMilliseconds < 10) {
//         count++;
//         var hash = Guid.NewGuid().ToString("N");
//         var svg = ident.Generate(hash);
//         // File.WriteAllText($"identicons/{hash}.svg", svg);
//     }
//
//     Console.WriteLine($"Generated {count} identicons in {sw.Elapsed}");
//     await Task.Delay(1000);
// }

// ident.Generate("asdf");

List<string> values = [
    "Zoey",
    "Willow",
    "Whiskers",
    "Zoe",
    "Sam",
    "Salem",
    "Snickers",
    "Snowball",
    "Angel",
    "Simon",
    "Charlie",
    "Daisy",
    "Sasha",
    "Samantha",
    "Sammy",
    "Dusty",
    "Sassy",
    "Annie",
    "Felix",
    "Pepper"
];

var ident2 = new SvgIdenticonGenerator();
foreach (var value in values) {
    var svg = ident2.Generate(value);
    File.WriteAllText($"identicons/{values.IndexOf(value)} - {value}.svg", svg);
}