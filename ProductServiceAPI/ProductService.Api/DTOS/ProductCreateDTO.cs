namespace ProductService.Api.DTOS;
public class ProductCreateDTO
{
    public required string Name { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
}