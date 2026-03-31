public interface IOrderClient
{
    Task<bool> OrderExistsAsync(int orderId);
}