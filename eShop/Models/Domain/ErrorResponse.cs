
namespace eShop.Models.Domain;

public class ErrorResponse
{
    public required string Error { get; set; }
    public string? Details { get; set; }
}
