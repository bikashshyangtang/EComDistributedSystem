using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

public class RabbitMqCancelConsumer: BackgroundService
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IConfiguration _config;

    public RabbitMqCancelConsumer(IServiceScopeFactory scopeFactory, IConfiguration config)
    {
        _scopeFactory = scopeFactory;
        _config = config;
        var factory = new ConnectionFactory
        {
            HostName = _config["RabbitMQ:Host"] ?? "rabbitmq",
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
        _channel.QueueDeclare(queue: "order_cancelled_queue", durable: false, exclusive: false, autoDelete: false, arguments: null);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += async (model, ea) =>
        {  
            var orderCancelledEvent = JsonSerializer.Deserialize<OrderCancelledEvent>(Encoding.UTF8.GetString(ea.Body.ToArray()));
            if(orderCancelledEvent != null)
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<PaymentDbContext>();
                    // Logic to update payment record based on order cancellation details
                    var payment = dbContext.Payments.FirstOrDefault(p => p.OrderId == orderCancelledEvent.OrderId);
                    if(payment != null)
                    {
                        payment.Status = "Refunded";
                        await dbContext.SaveChangesAsync();
                        Console.WriteLine($"Payment record updated for Order ID: {orderCancelledEvent.OrderId}, Status: Refunded");

                        // Optionally, you can publish a payment refund event to another queue
                        var publisher = scope.ServiceProvider.GetRequiredService<RabbitMqPayPublisher>();
                        publisher.PublishPaymentRefund(new PaymentRefundEvent
                        {
                            OrderId = orderCancelledEvent.OrderId,
                            PaymentId = payment.Id,
                            Status = payment.Status,
                            TotalAmount = payment.Amount,
                            CustomerId = payment.CustomerId,
                            ProductId = payment.ProductId
                        });
                    }
                }
            }
        };
        _channel.BasicConsume(queue: "order_cancelled_queue", autoAck: true, consumer: consumer);
        return Task.CompletedTask;
    }
    public override void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
        base.Dispose();
    }
}