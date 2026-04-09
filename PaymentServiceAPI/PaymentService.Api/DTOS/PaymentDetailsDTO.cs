namespace PaymentService.Api.DTOS;
public class PaymentDetailsDTO
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public decimal Amount { get; set; }
    public required string Status { get; set; }
}