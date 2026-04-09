using System.Net.Http;
using System.Text.Json;
using ApiGateway.Gateway.Api.DTOS;

namespace ApiGateway.Gateway.Api.Services;
public class OrderClient : IOrderClient
{
    private readonly HttpClient _httpClient;

    public OrderClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<OrderAggregateDTO?> GetOrderByIdAsync(int orderId)
    {
        var response = await _httpClient.GetAsync($"/api/orders/{orderId}");
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<OrderAggregateDTO>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
        return null;
    }
}