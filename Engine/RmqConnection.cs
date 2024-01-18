using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Diagnostics;
using System.Text;

namespace RmqTesting.Engine;

public class RmqConnection : IDisposable
{
    private readonly IConnection _connection;
    private readonly IModel _model;
    private IBasicConsumer? _consumer;
    private bool disposedValue;

    public RmqConnection()
    {
        ConnectionFactory factory = new()
        {
            HostName = "127.0.0.1",
        };

        try
        {
            _connection = factory.CreateConnection();
            _model = _connection.CreateModel();
            Declare();
        }
        catch
        {
            _model?.Close();
            _model?.Dispose();
            _connection?.Close();
            _connection?.Dispose();
            throw;
        }
    }

    public void Send(string message)
    {
        if (disposedValue) return;
        if (!_connection.IsOpen) return;

        ReadOnlyMemory<byte> body = Encoding.UTF8.GetBytes(message);

        _model.BasicPublish(string.Empty, "foo", null, body);
    }

    public bool Consume(EventHandler<BasicDeliverEventArgs> handler)
    {
        if (disposedValue || !_connection.IsOpen)
        {
            return false;
        }

        if (_consumer is null)
        {
            _consumer = new EventingBasicConsumer(_model);

            Dictionary<string, object> arguments = new()
            {
                { "x-stream-offset", "first" }
            };

            _model.BasicQos(prefetchSize: 0, prefetchCount: 10, global: false);
            _model.BasicConsume(_consumer, queue: "foo", arguments: arguments);
        }

        (_consumer as EventingBasicConsumer)!.Received += handler;

        return true;
    }

    public void Ack(ulong deliveryTag)
    {
        _model.BasicAck(deliveryTag, multiple: false);
    }

    public void Reset()
    {
        _model.QueueDelete(queue: "foo");
        Declare();
    }

    private void Declare()
    {
        Dictionary<string, object> arguments = new()
        {
            { "x-queue-type", "stream" },
        };
        _model.QueueDeclare(queue: "foo", durable: true, exclusive: false, autoDelete: false, arguments);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                _model?.Close();
                _model?.Dispose();
                _connection?.Close();
                _connection?.Dispose();
            }
            disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
