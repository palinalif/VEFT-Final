namespace ShroomCity.Models.Dtos;

public class UserDto
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Email { get; set; } = "";
    public string? Bio { get; set; }
    public IEnumerable<string> Permissions { get; set; } = new List<string>();
    public int TokenId { get; set; }
}