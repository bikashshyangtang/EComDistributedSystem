using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

public class RabbitMqPayPublisher : IDisposable
{
    private readonly IConnection _connection;
    private readonly IModel _channel;

    private const string PaymentProcessedQueueName = "payment_processed_queue";
    private const string PaymentRefundQueueName = "payment_refund_queue";


    public RabbitMqPayPublisher(IConfiguration config)
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
        _channel.QueueDeclare(queue: PaymentProcessedQueueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
        _channel.QueueDeclare(queue: PaymentRefundQueueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
    }

    public void PublishPaymentProcessed(PaymentProcessedEvent paymentProcessedEvent)
    {
        PublishEvent(PaymentProcessedQueueName, paymentProcessedEvent);
    }

    public void PublishPaymentRefund(PaymentRefundEvent paymentRefundEvent)
    {
        PublishEvent(PaymentRefundQueueName, paymentRefundEvent);
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