using RmqTesting.Engine;
using System.Diagnostics;

var messageCount = args.Length > 0 && int.TryParse(args[0], out var parsedCount)
    ? parsedCount 
    : 10;

using RmqConnection connection = new();

Stopwatch stopwatch = Stopwatch.StartNew();

for (int i = 0; i < messageCount; i++)
{
    connection.Send($"DEBUG - This is message {i} from {Environment.ProcessId}.");
}

stopwatch.Stop();
connection.Send($"INFO - {Environment.ProcessId} sent {messageCount} messages in {stopwatch.ElapsedMilliseconds} ms.");