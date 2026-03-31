public class Order
{
    public int Id{get; set;}
    public int CustomerId { get; set; }
    public decimal TotalAmount { get; set; }
    public bool IsDelivered { get; set; } = false;
    public int ProductId { get; set; }
}