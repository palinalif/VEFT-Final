namespace ShroomCity.Models.Dtos;

public class ResearcherDto
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string EmailAddress { get; set; } = "";
    public string? Bio { get; set; }
    public IEnumerable<MushroomDto>? AssociatedMushrooms { get; set; }
}