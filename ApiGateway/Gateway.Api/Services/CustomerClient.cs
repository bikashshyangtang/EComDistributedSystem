using System.Net.Http;
using System.Text.Json;
using ApiGateway.Gateway.Api.DTOS;

namespace ApiGateway.Gateway.Api.Services;
public class CustomerClient : ICustomerClient
{
    private readonly HttpClient _httpClient;

    public CustomerClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<CustomerAggregateDTO?> GetCustomerByIdAsync(int customerId)
    {
        var response = await _httpClient.GetAsync($"/api/customers/{customerId}");
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<CustomerAggregateDTO>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
        return null;
    }
}