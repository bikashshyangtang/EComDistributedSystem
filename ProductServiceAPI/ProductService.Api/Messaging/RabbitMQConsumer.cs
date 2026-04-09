using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

public class RabbitMQConsumer : BackgroundService
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly string _queueName;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IConfiguration _config;


    public RabbitMQConsumer(IServiceScopeFactory scopeFactory, IConfiguration config)
    {
        _scopeFactory = scopeFactory;
        _config = config;
        _queueName = "payment_processed_queue";

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
        _channel.QueueDeclare(queue: _queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            Console.WriteLine($"Message received from queue: {_queueName}");
            Console.WriteLine($"Message content: {message}");
            // Here you can add logic to process the message and update the product stock accordingly
            var paymentProcessedEvent = JsonSerializer.Deserialize<PaymentProcessedEvent>(message);
            if(paymentProcessedEvent != null)
            {
                using var scope = _scopeFactory.CreateScope();
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<ProductDbContext>();
                    Console.WriteLine($"Processing Order ID: {paymentProcessedEvent.OrderId}, Product ID: {paymentProcessedEvent.ProductId}, Customer ID: {paymentProcessedEvent.CustomerId}, Total Amount: {paymentProcessedEvent.TotalAmount}");
                    //Logic to update product stock based on order details
                    var product = await dbContext.Products.FindAsync(paymentProcessedEvent.ProductId);
                    var InitialStock = product?.InventoryStock ?? 0;
                    if(product != null && product.InventoryStock > 0)
                    {
                        product.InventoryStock -= 1; // Assuming each order reduces stock by 1, adjust as necessary
                        await dbContext.SaveChangesAsync();
                        Console.WriteLine($"Product ID: {product.Id} stock updated. Remaining stock: {product.InventoryStock}");
                    }
                    else
                    {
                        Console.WriteLine($"Product with ID: {paymentProcessedEvent.ProductId} not found.");
                    }
                }  
            }
        };
        _channel.BasicConsume(queue: _queueName, autoAck: true, consumer: consumer);
        return Task.CompletedTask;
    }

    public override void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
        base.Dispose();
    }
}