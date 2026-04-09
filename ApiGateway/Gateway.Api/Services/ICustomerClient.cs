using ApiGateway.Gateway.Api.DTOS;
namespace ApiGateway.Gateway.Api.Services;

public interface ICustomerClient
{
    Task<CustomerAggregateDTO?> GetCustomerByIdAsync(int customerId);
}