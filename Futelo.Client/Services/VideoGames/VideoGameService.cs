using System.Net.Http.Json;
using Futelo.Shared.DTOs.VideoGame;

namespace Futelo.Client.Services.VideoGames;

public class VideoGameService(HttpClient http) : IVideoGameService
{
    public async Task<List<VideoGameResponse>> GetAllAsync()
        => await http.GetFromJsonAsync<List<VideoGameResponse>>("api/videogames") ?? [];

    public async Task<VideoGameResponse> CreateAsync(CreateVideoGameRequest request)
    {
        var response = await http.PostAsJsonAsync("api/videogames", request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<VideoGameResponse>()
            ?? throw new InvalidOperationException("Invalid server response.");
    }

    public async Task UpdateAsync(int id, CreateVideoGameRequest request)
    {
        var response = await http.PutAsJsonAsync($"api/videogames/{id}", request);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteAsync(int id)
    {
        var response = await http.DeleteAsync($"api/videogames/{id}");
        response.EnsureSuccessStatusCode();
    }
}
