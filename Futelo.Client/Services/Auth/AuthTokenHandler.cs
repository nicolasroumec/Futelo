using System.Net.Http.Headers;

namespace Futelo.Client.Services.Auth;

public class AuthTokenHandler(FuteloAuthStateProvider authProvider) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrEmpty(authProvider.CurrentToken))
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", authProvider.CurrentToken);

        return await base.SendAsync(request, cancellationToken);
    }
}
