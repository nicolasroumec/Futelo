using System.Net.Http.Json;
using Futelo.Shared.DTOs.VideoGame;

namespace Futelo.Client.Services.VideoGames;

public class VideoGameService(HttpClient http) : IVideoGameService
{
    public async Task<List<VideoGameResponse>> GetAllAsync()
        => await http.GetFromJsonAsync<List<VideoGameResponse>>("api/videogames") ?? [];
}
