using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

public class RabbitMQPublisher: IDisposable
{
    private readonly IConnection _connection;
    private readonly IModel _channel;

    private const string OrderCreatedQueueName = "order_queue";
    private const string OrderCancelledQueueName = "order_cancelled_queue";

    public RabbitMQPublisher(IConfiguration config)
    {
        var factory = new ConnectionFactory
        {
            HostName = config["RabbitMQ:Host"] ?? "rabbitmq",
            UserName = "guest",
            Password = "guest"
        };
        IConnection? connection = null;
        int retries = 10;
        while (retries-- > 0)
        {
            try { connection = factory.CreateConnection(); break; }
            catch { Console.WriteLine("RabbitMQConsumer: retrying in 3s..."); Thread.Sleep(3000); }
        }
        _connection = connection ?? throw new Exception("Could not connect to RabbitMQ.");
        _channel = _connection.CreateModel();
        _channel.QueueDeclare(queue: OrderCreatedQueueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
        _channel.QueueDeclare(queue: OrderCancelledQueueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
    }

    public void PublishOrderCreatedEvent(OrderCreatedEvent orderCreatedEvent)
    {
       PublishEvent(OrderCreatedQueueName, orderCreatedEvent);
    }

    public void PublishOrderCancelledEvent(OrderCancelledEvent orderCancelledEvent)
    {
        PublishEvent(OrderCancelledQueueName, orderCancelledEvent);
    }

    private void PublishEvent(string queueName, object eventObject)
    {
        var message = JsonSerializer.Serialize(eventObject);
        var body = Encoding.UTF8.GetBytes(message);
        _channel.BasicPublish(exchange: "", routingKey: queueName, basicProperties: null, body: body);
        Console.WriteLine($"Message published to queue: {queueName}");
    }

    public void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
    }
}