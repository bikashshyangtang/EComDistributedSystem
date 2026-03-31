public interface IProductClient
{
    Task<bool> ProductStockUpdateAsync(int productId);
}