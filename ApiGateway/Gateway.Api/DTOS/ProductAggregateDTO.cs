namespace ApiGateway.Gateway.Api.DTOS;
public class ProductAggregateDTO 
{ 
    public int Id{get;set;} 
    public string? Name{get;set;} 
    public decimal Price{get;set;} 
    public int InventoryStock{get;set;}
    public bool IsAvailable{get;set;}
}