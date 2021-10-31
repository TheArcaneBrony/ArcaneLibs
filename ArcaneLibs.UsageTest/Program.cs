// See https://aka.ms/new-console-template for more information

using ArcaneLibs.Logging;
using ArcaneLibs.Logging.LogEndpoints;

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