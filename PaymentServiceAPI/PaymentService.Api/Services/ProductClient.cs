using System.Net;
using System.Threading.Tasks;  
public class ProductClient : IProductClient
{
    private readonly HttpClient _httpClient;

    public ProductClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<bool> ProductStockUpdateAsync(int productId)
    {
        var response = await _httpClient.PutAsync($"api/products/reduceStock/{productId}", null);
        return response.StatusCode == HttpStatusCode.OK;
    }
}