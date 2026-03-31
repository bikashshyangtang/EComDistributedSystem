using System.Net;
using System.Threading.Tasks;
public class OrderClient : IOrderClient
{
    private readonly HttpClient _httpClient;

    public OrderClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<bool> OrderExistsAsync(int orderId)
    {
        var response = await _httpClient.GetAsync($"api/orders/{orderId}");
        return response.StatusCode == HttpStatusCode.OK;
    }
}