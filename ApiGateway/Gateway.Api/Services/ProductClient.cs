using System.Net.Http;
using System.Text.Json;
using ApiGateway.Gateway.Api.DTOS;

namespace ApiGateway.Gateway.Api.Services;
public class ProductClient : IProductClient
{
    private readonly HttpClient _httpClient;

    public ProductClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ProductAggregateDTO?> GetProductByIdAsync(int productId)
    {
        var response = await _httpClient.GetAsync($"/api/products/{productId}");
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ProductAggregateDTO>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
        return null;
    }
}