namespace eShop.Models.Dtos;

public class Send2faCodeDto
{
    public required string UserName { get; set; }
    public string? Provider { get; set; }
}