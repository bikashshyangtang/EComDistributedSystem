namespace ApiGateway.Gateway.Api.DTOS;
public class OrderAggregateDTO 
{ 
    public int Id{get;set;} 
    public int CustomerId{get;set;} 
    public int ProductId{get;set;} 
    public decimal TotalAmount{get;set;} 
    public bool IsDelivered{get;set;}
    public bool IsCancelled { get; set; }
}