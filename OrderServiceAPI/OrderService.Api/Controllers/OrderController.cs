using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using OrderService.Api.DTOS;

[ApiController]
[Route("api/orders")]
public class OrderController : ControllerBase
{
    private readonly OrderDbContext _context;
    private readonly ICustomerClient _customerClient;
    private readonly IProductClient _productClient;
    private readonly RabbitMQPublisher _rabbitMQPublisher;

    public OrderController(OrderDbContext context, ICustomerClient customerClient, IProductClient productClient, RabbitMQPublisher rabbitMQPublisher)
    {
        _context = context;
        _customerClient = customerClient;
        _productClient = productClient;
        _rabbitMQPublisher = rabbitMQPublisher;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var orderResult = new List<OrderCreateResponse>();
        var orders = await _context.Orders.ToListAsync();
        foreach (var order in orders)
        {
            var orderResponse = new OrderCreateResponse
            {
                Id = order.Id,
                CustomerId = order.CustomerId,
                TotalAmount = order.TotalAmount,
                IsDelivered = order.IsDelivered,
                ProductId = order.ProductId,
                IsCancelled = order.IsCancelled
            };
            orderResult.Add(orderResponse);
        }
        return Ok(orderResult);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetOrder(int id)
    {
        var order = await _context.Orders.FindAsync(id);
        if (order == null)
        {
            return NotFound();
        }
        var orderResponse = new OrderCreateResponse
        {
            Id = order.Id,
            CustomerId = order.CustomerId,
            TotalAmount = order.TotalAmount,
            IsDelivered = order.IsDelivered,
            ProductId = order.ProductId,
            IsCancelled = order.IsCancelled
        };
        return Ok(orderResponse);
    }

    [HttpPost]
    public async Task<IActionResult> Create(OrderCreateRequest orderDto)
    {
        var order = new Order
        {
            CustomerId = orderDto.CustomerId,
            ProductId = orderDto.ProductId,
            TotalAmount = orderDto.TotalAmount
        };
        var customerExists = await _customerClient.CustomerExistsAsync(order.CustomerId);
        if (!customerExists)
        {
            return BadRequest($"Customer does not exist.");
            //return NotFound($"Customer with ID {order.CustomerId} does not exist.");
        }
        var productAvailable = await _productClient.ProductAvailableAsync(order.ProductId);
        if (!productAvailable)
        {
            return BadRequest($"Product is not available.");
        }
        await _context.Orders.AddAsync(order);
        await _context.SaveChangesAsync();
        var orderCreatedEvent = new OrderCreatedEvent
        {
            OrderId = order.Id,
            CustomerId = order.CustomerId,
            ProductId = order.ProductId,
            TotalAmount = order.TotalAmount
        };
       // var orderCreatedEventJson = JsonSerializer.Serialize(orderCreatedEvent);
        _rabbitMQPublisher.PublishOrderCreatedEvent(orderCreatedEvent);
        var orderResponse = new OrderCreateResponse
        {
            Id = order.Id,
            CustomerId = order.CustomerId,
            TotalAmount = order.TotalAmount,
            IsDelivered = order.IsDelivered,
            ProductId = order.ProductId,
            IsCancelled = order.IsCancelled
        };
        return Ok(orderResponse);
    }

    [HttpGet("trackOrder/{Id}")]
    public async Task<IActionResult> TrackOrder(int Id)
    {
        var order = await _context.Orders.FindAsync(Id);
        if (order == null)
        {
            return NotFound($"Order does not exist.");
        }
        if (order.IsDelivered)
        {
            return Ok($"Order has been delivered.");
        }
        else
        {
            return Ok($"Order is still being processed.");
        }
    }

    [HttpPut("cancelOrder/{id}")]
    public async Task<IActionResult> CancelOrder(int id)
    {
        var order = await _context.Orders.FindAsync(id);
        if (order == null)
        {
            return NotFound($"Order does not exist.");
        }
        if (order.IsDelivered)
        {
            return BadRequest($"Order has already been delivered and cannot be cancelled.");
        }
        order.IsCancelled = true;
        await _context.SaveChangesAsync();
        var orderCancelledEvent = new OrderCancelledEvent
        {
            OrderId = order.Id,
            CustomerId = order.CustomerId,
            ProductId = order.ProductId,
            TotalAmount = order.TotalAmount
        };
        //var orderCancelledEventJson = JsonSerializer.Serialize(orderCancelledEvent);
        _rabbitMQPublisher.PublishOrderCancelledEvent(orderCancelledEvent);
        return Ok($"Order has been cancelled.");
    }
}

