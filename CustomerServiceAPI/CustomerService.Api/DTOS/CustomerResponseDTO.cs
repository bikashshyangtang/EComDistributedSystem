namespace CustomerService.Api.DTOS;
public class CustomerResponseDTO
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }
}