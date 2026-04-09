using System.Net.Http;
using System.Text.Json; 
using ApiGateway.Gateway.Api.DTOS;

namespace ApiGateway.Gateway.Api.Services;
public interface IOrderClient
{
    Task<OrderAggregateDTO?> GetOrderByIdAsync(int orderId);
}