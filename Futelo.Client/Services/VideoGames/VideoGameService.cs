using Futelo.Shared.DTOs.VideoGame;

namespace Futelo.Client.Services.VideoGames;

public class VideoGameService(HttpClient http) : ApiService(http), IVideoGameService
{
    public Task<List<VideoGameResponse>> GetAllAsync()
        => GetListAsync<VideoGameResponse>("api/videogames");

    public Task<VideoGameResponse> CreateAsync(CreateVideoGameRequest request)
        => PostAsync<VideoGameResponse>("api/videogames", request);

    public Task UpdateAsync(int id, CreateVideoGameRequest request)
        => PutAsync($"api/videogames/{id}", request);

    public Task DeleteAsync(int id)
        => DeleteAsync($"api/videogames/{id}");
}
