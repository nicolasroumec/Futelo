using System.Net.Http.Json;

namespace Futelo.Client.Services;

public abstract class ApiService(HttpClient http)
{
    protected readonly HttpClient Http = http;

    protected async Task<T> GetAsync<T>(string url)
        => await Http.GetFromJsonAsync<T>(url)
            ?? throw new InvalidOperationException("Empty server response.");

    protected async Task<List<T>> GetListAsync<T>(string url)
        => await Http.GetFromJsonAsync<List<T>>(url) ?? [];

    protected async Task<T> PostAsync<T>(string url, object body)
    {
        var res = await Http.PostAsJsonAsync(url, body);
        await res.EnsureSuccessAsync();
        return await res.Content.ReadFromJsonAsync<T>()
            ?? throw new InvalidOperationException("Empty server response.");
    }

    protected async Task PostAsync(string url)
    {
        var res = await Http.PostAsync(url, null);
        await res.EnsureSuccessAsync();
    }

    protected async Task PutAsync(string url, object? body = null)
    {
        var res = body is null
            ? await Http.PutAsync(url, null)
            : await Http.PutAsJsonAsync(url, body);
        await res.EnsureSuccessAsync();
    }

    protected async Task<T> PutAsync<T>(string url, object body)
    {
        var res = await Http.PutAsJsonAsync(url, body);
        await res.EnsureSuccessAsync();
        return await res.Content.ReadFromJsonAsync<T>()
            ?? throw new InvalidOperationException("Empty server response.");
    }

    protected async Task PatchAsync(string url, object body)
    {
        var res = await Http.PatchAsJsonAsync(url, body);
        await res.EnsureSuccessAsync();
    }

    protected async Task<T> PatchAsync<T>(string url, object body)
    {
        var res = await Http.PatchAsJsonAsync(url, body);
        await res.EnsureSuccessAsync();
        return await res.Content.ReadFromJsonAsync<T>()
            ?? throw new InvalidOperationException("Empty server response.");
    }

    protected async Task DeleteAsync(string url)
    {
        var res = await Http.DeleteAsync(url);
        await res.EnsureSuccessAsync();
    }
}
