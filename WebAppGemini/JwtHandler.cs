using System.Net.Http.Headers;
using Blazored.LocalStorage;

public class JwtHandler : DelegatingHandler
{
    private readonly ILocalStorageService _localStorage;

    public JwtHandler(ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        // 1. Prendi il token dal LocalStorage
        var token = await _localStorage.GetItemAsync<string>("authToken");

        // 2. Se esiste, aggiungilo all'header della richiesta
        if (!string.IsNullOrEmpty(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        // 3. Continua la richiesta
        return await base.SendAsync(request, cancellationToken);
    }
}