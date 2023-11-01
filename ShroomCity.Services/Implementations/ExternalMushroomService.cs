using System.Text.Json;
using ShroomCity.Models;
using ShroomCity.Models.Dtos;
using ShroomCity.Services.Interfaces;

namespace ShroomCity.Services.Implementations;

public class ExternalMushroomService : IExternalMushroomService
{
    static readonly HttpClient client = new HttpClient();
    public async Task<ExternalMushroomDto?> GetMushroomByName(string name)
    {
        string responseBody = await client.GetStringAsync($"https://mushrooms-api-a309dd19945c.herokuapp.com/mushrooms/{name}");
        var mushroom = JsonSerializer.Deserialize<ExternalMushroomDto>(responseBody);
        return mushroom;
    }

    public async Task<Envelope<ExternalMushroomDto>?> GetMushrooms(int pageSize, int pageNumber)
    {
        string responseBody = await client.GetStringAsync($"https://mushrooms-api-a309dd19945c.herokuapp.com/mushrooms?pageNumber={pageNumber}&pageSize={pageSize}");
        var envelope = JsonSerializer.Deserialize<Envelope<ExternalMushroomDto>>(responseBody);
        return envelope;
    }
}