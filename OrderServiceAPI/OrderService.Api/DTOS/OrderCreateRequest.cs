namespace OrderService.Api.DTOS;
public class OrderCreateRequest
{
    public int CustomerId { get; set; }
    public decimal TotalAmount { get; set; }
    public int ProductId { get; set; }
}