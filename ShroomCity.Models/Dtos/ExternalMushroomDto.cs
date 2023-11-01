using System.Text.Json.Serialization;

namespace ShroomCity.Models.Dtos;

public class ExternalMushroomDto
{
    [JsonPropertyName("_id")]
    public string Id { get; set; } = "";
    [JsonPropertyName("name")]
    public string Name { get; set; } = "";
    [JsonPropertyName("description")]
    public string Description { get; set; } = "";
    [JsonPropertyName("colors")]
    public IEnumerable<string> Colors { get; set; } = new List<string>();
    [JsonPropertyName("shapes")]
    public IEnumerable<string> Shapes { get; set; } = new List<string>();
    [JsonPropertyName("surfaces")]
    public IEnumerable<string> Surfaces { get; set; } = new List<string>();
}