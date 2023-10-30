namespace ShroomCity.Models.Dtos;

public class ExternalMushroomDto
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public IEnumerable<string> Colors { get; set; } = new List<string>();
    public IEnumerable<string> Shapes { get; set; } = new List<string>();
    public IEnumerable<string> Surfaces { get; set; } = new List<string>();
}