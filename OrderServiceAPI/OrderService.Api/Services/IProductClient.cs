public interface IProductClient
{
    Task<bool> ProductAvailableAsync(int productId);
}