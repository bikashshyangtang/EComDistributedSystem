using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using PaymentService.Api.DTOS;

[ApiController]
[Route("api/payments")]
public class PaymentController : ControllerBase
{
    private readonly PaymentDbContext _context;
    private readonly IProductClient _productClient;
    private readonly IOrderClient _orderClient;
    private readonly RabbitMqPayPublisher _rabbitMQPublisher;
    public PaymentController(PaymentDbContext context, IProductClient productClient, IOrderClient orderClient, RabbitMqPayPublisher rabbitMQPublisher)
    {
        _context = context;
        _productClient = productClient;
        _orderClient = orderClient;
        _rabbitMQPublisher = rabbitMQPublisher;
    }

    [HttpGet]
    public async Task<IActionResult> GetDetails()
    {
        var paymentList = new List<PaymentDetailsDTO>();
        var payments = await _context.Payments.ToListAsync();
        foreach (var payment in payments)
        {
            var paymentDetails = new PaymentDetailsDTO
            {
                Id = payment.Id,
                OrderId = payment.OrderId,
                Amount = payment.Amount,
                Status = payment.Status
            };
            paymentList.Add(paymentDetails);
        }
        return Ok(paymentList);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetPaymentById(int id)
    {
        var payment = await _context.Payments.FindAsync(id);
        if (payment == null)
        {
            return NotFound();
        }
        var paymentDetails = new PaymentDetailsDTO
        {
            Id = payment.Id,
            OrderId = payment.OrderId,
            Amount = payment.Amount,
            Status = payment.Status
        };
        return Ok(paymentDetails);
    }
}