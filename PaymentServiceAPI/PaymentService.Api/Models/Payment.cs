public class Payment
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public decimal Amount { get; set; }
    public int OrderId { get; set; }
    public  required string Status { get; set; }
    public int CustomerId{get; set;}
}