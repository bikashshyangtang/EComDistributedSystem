using Microsoft.AspNetCore.Mvc;
using ApiGateway.Gateway.Api.DTOS;
using ApiGateway.Gateway.Api.Services;
using System.Net.Http;


[ApiController]
[Route("gateway/aggregate")]
public class AggregationController : ControllerBase
{
    private readonly HttpClient _httpClient;
    private readonly IProductClient _productClient;
    private readonly IOrderClient _orderClient;
    private readonly ICustomerClient _customerClient;

    public AggregationController(IHttpClientFactory httpClient, IProductClient productClient, IOrderClient orderClient, ICustomerClient customerClient)
    {
        _httpClient = httpClient.CreateClient();
        _productClient = productClient;
        _orderClient = orderClient;
        _customerClient = customerClient;
    }

    [HttpGet("orderdetails/{orderId}")]
    public async Task<IActionResult> GetOrderDetails(int orderId)
    {
        var order = await _orderClient.GetOrderByIdAsync(orderId);
        if (order == null)
        {
            return NotFound();
        }
        var resultOrder = new OrderAggregateDTO
        {
            Id = order.Id,
            TotalAmount = order.TotalAmount,
            IsDelivered = order.IsDelivered,
            IsCancelled = order.IsCancelled,
            CustomerId = order.CustomerId,
            ProductId = order.ProductId
        };
        var product = await _productClient.GetProductByIdAsync(order.ProductId);
        if (product == null)
        {
            return NotFound();
        }
        var resultProduct = new ProductAggregateDTO
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            IsAvailable = product.IsAvailable
        };
        var customer = await _customerClient.GetCustomerByIdAsync(order.CustomerId);
        if (customer == null)
        {
            return NotFound();
        }
        var resultCustomer = new CustomerAggregateDTO
        {
            Id = customer.Id,
            Name = customer.Name,
            Email = customer.Email
        };
        return Ok(new { Order = resultOrder, Product = resultProduct, Customer = resultCustomer });
    }
}