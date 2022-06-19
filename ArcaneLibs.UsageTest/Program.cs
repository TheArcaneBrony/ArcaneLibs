// See https://aka.ms/new-console-template for more information

using System;
using System.Drawing;
using ArcaneLibs;
using ArcaneLibs.Collections;
using ArcaneLibs.Logging;
using ArcaneLibs.Logging.LogEndpoints;
using ArcaneLibs.UsageTest;

Console.WriteLine("Hello, World!");
//create logs
LogManager log1 = new LogManager();
LogManager log2 = new LogManager();
LogManager log3 = new LogManager();
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

for (int i = 0; i < 5; i++) {
    Config myConfig = Config.Read();
    Console.WriteLine(myConfig.SomeNumber);
    myConfig.SomeNumber = i;
    myConfig.Save();
    myConfig = Config.Read();
    Console.WriteLine(myConfig.SomeNumber);
}
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
var autodict = new AutoPopulatingDictionary<int, String>();
var autodict2 = new AutoPopulatingDictionary<int, Config>();
var autodict3 = new AutoPopulatingDictionary<int, Point>();

var a = autodict[3];
var b = autodict2[5];
var c = autodict3[6];

//return;

Console.WriteLine(Util.GetCommandOutputSync("bash", "-c asdf"));
Console.WriteLine(Util.GetCommandOutputSync("bash", "-c ls"));

