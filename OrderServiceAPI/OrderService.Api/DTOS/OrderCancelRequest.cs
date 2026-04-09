
namespace OrderService.Api.DTOS;

public class OrderCancelRequest
{
    public int OrderId { get; set; }
    public int ProductId { get; set; }
    public int CustomerId { get; set; }
    public decimal TotalAmount { get; set; }
}