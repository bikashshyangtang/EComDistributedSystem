namespace ProductService.Api.DTOS;
public class ProductDetailDTO
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
}