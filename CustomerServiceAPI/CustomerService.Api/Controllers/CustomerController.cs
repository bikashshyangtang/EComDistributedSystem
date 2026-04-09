using Microsoft.AspNetCore.Mvc;
using CustomerService.Api.DTOS;
using Microsoft.EntityFrameworkCore;


[ApiController]
[Route("api/customers")]
public class CustomerController : ControllerBase
{
    private readonly CustomerDbContext _context;

    public CustomerController(CustomerDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllCustomers()
    {
        var customers = await _context.Customers.ToListAsync();
        var result = new List<CustomerResponseDTO>();
        foreach (var customer in customers)
        {
            var customerDto = new CustomerResponseDTO
            {
                Id = customer.Id,
                Name = customer.Name,
                Email = customer.Email,
            };
            result.Add(customerDto);
        }
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCustomerById(int id)
    {
        var customer = await _context.Customers.FindAsync(id);
        if (customer == null)
        {
            return NotFound();
        }
        var customerDto = new CustomerResponseDTO
        {
            Id = customer.Id,
            Name = customer.Name,
            Email = customer.Email,
        };
        return Ok(customerDto);
    }

    [HttpPost]
    public async Task<IActionResult> CreateCustomer(CustomerCreateDTO customerDto)
    {
        var customer = new Customer
        {
            Name = customerDto.Name,
            Email = customerDto.Email,
        };
        await _context.Customers.AddAsync(customer);
        await _context.SaveChangesAsync();
        var resultDto = new CustomerResponseDTO
        {
            Id = customer.Id,
            Name = customer.Name,
            Email = customer.Email
        };
        return Ok(resultDto);
    }
}