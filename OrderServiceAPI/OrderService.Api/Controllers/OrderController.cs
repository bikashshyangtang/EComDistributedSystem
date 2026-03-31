using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/orders")]
public class OrderController : ControllerBase
{
    private readonly OrderDbContext _context;
    private readonly ICustomerClient _customerClient;
    private readonly IProductClient _productClient;

    public OrderController(OrderDbContext context, ICustomerClient customerClient, IProductClient productClient)
    {
        _context = context;
        _customerClient = customerClient;
        _productClient = productClient;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var orders = await _context.Orders.ToListAsync();
        return Ok(orders);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetOrder(int id)
    {
        var order = await _context.Orders.FindAsync(id);
        if (order == null)
        {
            return NotFound();
        }
        return Ok(order);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Order order)
    {
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
        //return CreatedAtAction(nameof(GetAll), new { id = order.Id }, order);
        return Ok(order);
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

}

