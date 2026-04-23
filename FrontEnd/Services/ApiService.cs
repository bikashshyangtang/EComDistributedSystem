using System.Net.Http.Json;
using FrontEnd.Models;

namespace FrontEnd.Services;

public class ApiService
{
    private readonly HttpClient _httpClient;

    public ApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    // ─── Products ─────────────────────────────────────────────────────────────

    public async Task<List<ProductResponseDto>> GetProductsAsync()
    {
        var response = await _httpClient.GetAsync("gateway/products");
        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync();
            throw new Exception($"API error {response.StatusCode}: {body}");
        }
        return await response.Content.ReadFromJsonAsync<List<ProductResponseDto>>() ?? new List<ProductResponseDto>();
    }

    public async Task AddProductAsync(ProductDto product)
    {
        var response = await _httpClient.PostAsJsonAsync("gateway/products", product);
        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync();
            throw new Exception($"API error {response.StatusCode}: {body}");
        }
    }

    public async Task UpdateProductAsync(ProductResponseDto product)
    {
        var response = await _httpClient.PutAsJsonAsync($"gateway/products/{product.Id}", product);
        if (!response.IsSuccessStatusCode)
            throw new Exception($"API error {response.StatusCode}: {await response.Content.ReadAsStringAsync()}");
    }

    // ─── Customers ────────────────────────────────────────────────────────────

    public async Task<List<CustomerDto>> GetCustomersAsync()
    {
        var response = await _httpClient.GetAsync("gateway/customers");
        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync();
            throw new Exception($"API error {response.StatusCode}: {body}");
        }
        return await response.Content.ReadFromJsonAsync<List<CustomerDto>>() ?? new List<CustomerDto>();
    }

    public async Task AddCustomerAsync(CreateCustomerDto customer)
    {
        var response = await _httpClient.PostAsJsonAsync("gateway/customers", customer);
        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync();
            throw new Exception($"API error {response.StatusCode}: {body}");
        }
    }

    // ─── Orders ───────────────────────────────────────────────────────────────

    public async Task<List<OrderDto>> GetOrdersAsync()
    {
        var response = await _httpClient.GetAsync("gateway/orders");
        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync();
            throw new Exception($"API error {response.StatusCode}: {body}");
        }
        return await response.Content.ReadFromJsonAsync<List<OrderDto>>() ?? new List<OrderDto>();
    }

    public async Task<OrderDto?> CreateOrderAsync(OrderCreateDto order)
    {
        var response = await _httpClient.PostAsJsonAsync("gateway/orders", order);
        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync();
            throw new Exception($"API error {response.StatusCode}: {body}");
        }
        return await response.Content.ReadFromJsonAsync<OrderDto>();
    }
}
