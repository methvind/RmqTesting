# Setup

Make sure you have [.NET 8](https://dotnet.microsoft.com/en-us/download/dotnet/8.0) installed.

Make sure you have RabbitMQ installed. See [the downloads page](https://www.rabbitmq.com/download.html) for options (scroll down to the "Downloads on GitHub" section for the Windows installer).

Optionally, enable the management UI by first enabling the [management plugin](https://www.rabbitmq.com/management.html):

```console
rabbitmq-plugins enable rabbitmq_management
```

Then navigate to [the management UI at http://localhost:15672/](http://localhost:15672/) to see the status of your RabbitMQ service. The default credentials are `guest:guest` (username:password).

# Running

Build the solution.

```console
dotnet clean
dotnet build .\RmqTesting.sln
```

Run `Runner.exe` then supply an instance count (number of `Runtime.exe` processes) and message count (number of messages each instance will send to RabbitMQ).

```console
.\Runner\bin\Debug\net8.0\Runner.exe
```

![image](https://github.com/methvind/RmqTesting/assets/18388333/b5c7fc88-f4f3-4b32-97ef-e0ad6d242fd6)

Alternatively, run `Runtime.exe` directly by passing the message count as the only argument. This will not show any progress in the console, but it might be useful if you want to observe from the management UI.

```console
.\Runtime\bin\Debug\net8.0\Runtime.exe 10
```
