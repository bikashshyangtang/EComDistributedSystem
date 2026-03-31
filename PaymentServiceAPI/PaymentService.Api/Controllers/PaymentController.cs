using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/payments")]
public class PaymentController : ControllerBase
{
    private readonly PaymentDbContext _context;
    private readonly IProductClient _productClient;

    private readonly IOrderClient _orderClient;
    public PaymentController(PaymentDbContext context, IProductClient productClient, IOrderClient orderClient)
    {
        _context = context;
        _productClient = productClient;
        _orderClient = orderClient;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var payments = await _context.Payments.ToListAsync();
        return Ok(payments);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetPayment(int id)
    {
        var payment = await _context.Payments.FindAsync(id);
        if (payment == null)
        {
            return NotFound();
        }
        return Ok(payment);
    }

    [HttpPost]
    public async Task<IActionResult> ProcessPayment(Payment payment)
    {
        // Simulate payment processing logic
        if (payment.Amount <= 0)
        {
            return BadRequest("Invalid payment amount.");
        }
        var orderExists = await _orderClient.OrderExistsAsync(payment.ProductId);
        if (!orderExists)
        {
            return BadRequest("Order does not exist.");
        }
        // In a real application, you would integrate with a payment gateway here
        // Example: await _paymentGateway.ProcessPaymentAsync(payment);
        await _context.Payments.AddAsync(payment);
        await _context.SaveChangesAsync();
        var productStockUpdated = await _productClient.ProductStockUpdateAsync(payment.ProductId);
        if (!productStockUpdated)
        {
            return BadRequest($"Stock Update failed.");
        }
        return Ok();
    }
}