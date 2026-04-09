using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

public class RabbitMQRefundConsume: BackgroundService
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IConfiguration _config;

    public RabbitMQRefundConsume(IServiceScopeFactory scopeFactory, IConfiguration config)
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
        _channel.QueueDeclare(queue: "payment_refund_queue", durable: false, exclusive: false, autoDelete: false, arguments: null);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += async (model, ea) =>
        {
            var paymentRefundEvent = JsonSerializer.Deserialize<PaymentRefundEvent>(Encoding.UTF8.GetString(ea.Body.ToArray()));
            if(paymentRefundEvent != null)
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var productDbContext = scope.ServiceProvider.GetRequiredService<ProductDbContext>();
                    Console.WriteLine($"Payment record updated for Payment ID: {paymentRefundEvent.PaymentId}, Status: {paymentRefundEvent.Status}");
                    // Logic to update product stock based on refund details
                    var product = await productDbContext.Products.FindAsync(paymentRefundEvent.ProductId);
                    if(product != null)
                    {
                        product.InventoryStock += 1; // Assuming each refund increases stock by 1, adjust as necessary
                        await productDbContext.SaveChangesAsync();
                        Console.WriteLine($"Product ID: {product.Id} stock updated. Current stock: {product.InventoryStock}");
                    }
                }
            }  
            else
            {
                Console.WriteLine($"Payment with ID: {paymentRefundEvent?.PaymentId} not found.");
            }
        };
        _channel.BasicConsume(queue: "payment_refund_queue", autoAck: true, consumer: consumer);
        return Task.CompletedTask;
    }

    public override void Dispose()
    {
         
        _channel?.Dispose();
        _connection?.Dispose();
        base.Dispose();
     }
}