using System;

namespace FrontEnd.Models;
public class OrderDto
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public int ProductId { get; set; } = new();
    public decimal TotalAmount { get; set; }
    public bool IsDelivered { get; set; }
    public bool IsCancelled { get; set; }
}