using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

public class RabbitMqCreateConsumer: BackgroundService
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IConfiguration _config;


    public RabbitMqCreateConsumer(IServiceScopeFactory scopeFactory, IConfiguration config)
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
        _channel.QueueDeclare(queue: "order_queue", durable: false, exclusive: false, autoDelete: false, arguments: null);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += async (model, ea) =>
        {
            var orderCreatedEvent = JsonSerializer.Deserialize<OrderCreatedEvent>(Encoding.UTF8.GetString(ea.Body.ToArray()));
            if(orderCreatedEvent != null)
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<PaymentDbContext>();
                    // Logic to create payment record based on order details
                    var payment = new Payment
                    {
                        OrderId = orderCreatedEvent.OrderId,
                        CustomerId = orderCreatedEvent.CustomerId,
                        ProductId = orderCreatedEvent.ProductId,
                        Amount = orderCreatedEvent.TotalAmount,
                        Status = "Success"
                    };
                    await dbContext.Payments.AddAsync(payment);
                    await dbContext.SaveChangesAsync();
                    Console.WriteLine($"Payment record created for Order ID: {orderCreatedEvent.OrderId}, Amount: {orderCreatedEvent.TotalAmount}");

                    // Optionally, you can publish a payment processed event to another queue
                    var publisher = scope.ServiceProvider.GetRequiredService<RabbitMqPayPublisher>();
                    publisher.PublishPaymentProcessed(new PaymentProcessedEvent
                    {
                        OrderId = orderCreatedEvent.OrderId,
                        PaymentId = payment.Id,
                        Status = payment.Status,
                        TotalAmount = payment.Amount,
                        CustomerId = payment.CustomerId,
                        ProductId = payment.ProductId
                    });
                }
            }
        };
        _channel.BasicConsume(queue: "order_queue", autoAck: true, consumer: consumer);
        return Task.CompletedTask;
    }

    public override void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
        base.Dispose();
    }
}