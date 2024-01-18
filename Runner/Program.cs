using RmqTesting.Engine;
using System.Diagnostics;
using System.Text;

Console.WriteLine("Connecting...");

using RmqConnection connection = new();

connection.Reset();
connection.Consume((sender, args) =>
{
    connection.Ack(args.DeliveryTag);

    var response = Encoding.UTF8.GetString(args.Body.ToArray());

    if (response.StartsWith("INFO"))
    {
        Console.WriteLine(response);
    }
});

Console.WriteLine("Connected!");

int instanceCount;
int messageCountPerInstance;

do
{
    Console.WriteLine("Enter a number of instances to start:");
}
while (!int.TryParse(Console.ReadLine(), out instanceCount));

do
{
    Console.WriteLine("Enter a number of messages to send per instance:");
}
while (!int.TryParse(Console.ReadLine(), out messageCountPerInstance));

IEnumerable<Process> processes = Enumerable.Range(0, instanceCount)
    .Select(_ => new Process()
    {
        StartInfo = new()
        {
            FileName = @"F:\Projects\RmqTesting\Runtime\bin\Debug\net8.0\Runtime.exe",
            Arguments = $"{messageCountPerInstance}",
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        }
    });

foreach (var process in processes)
{
    process.Start();

    string error = process.StandardError.ReadToEnd();
    if (error != string.Empty)
    {
        Console.WriteLine($"Error running {process.Id}: {error}");
    }
}
