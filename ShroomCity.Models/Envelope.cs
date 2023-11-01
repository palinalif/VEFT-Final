using System.Text.Json.Serialization;

namespace ShroomCity.Models;

public class Envelope<T> where T : class
{
    [JsonPropertyName("pageSize")]
    public int PageSize { get; set; }
    [JsonPropertyName("pageNumber")]
    public int PageNumber { get; set; }
    [JsonPropertyName("totalPages")]
    public int TotalPages { get; set; }
    [JsonPropertyName("data")]
    public IEnumerable<T> Items { get; set; } = null!;
}