namespace eShop.Models.Dtos;

public class Verify2faDto
{
    public string UserName { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
}