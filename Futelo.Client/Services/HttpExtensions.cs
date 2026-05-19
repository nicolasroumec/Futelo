namespace Futelo.Client.Services;

public static class HttpExtensions
{
    public static async Task EnsureSuccessAsync(this HttpResponseMessage response, string fallback = "Operation failed.")
    {
        if (response.IsSuccessStatusCode) return;
        var body = await response.Content.ReadAsStringAsync();
        throw new InvalidOperationException(string.IsNullOrWhiteSpace(body) ? fallback : body);
    }
}
