public class PaymentProcessedEvent
{
    public int PaymentId { get; set; }
    public decimal TotalAmount { get; set; }
    public int OrderId { get; set; }
    public int CustomerId { get; set; }
    public required string Status { get; set; }
    public int ProductId { get; set; } // Assuming we need this to update product stock
}