using System.Net;
using System.Threading.Tasks;
public class ProductClient : IProductClient
{
    private readonly HttpClient _httpClient;

    public ProductClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<bool> ProductAvailableAsync(int productId)
    {
        var response = await _httpClient.GetAsync($"api/products/{productId}");
        return response.StatusCode == HttpStatusCode.OK;
    }
}