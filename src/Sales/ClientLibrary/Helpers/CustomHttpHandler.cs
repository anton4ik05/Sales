using System.Net;
using BaseLibrary.Dtos;
using ClientLibrary.Services.Contracts;

namespace ClientLibrary.Helpers;

public class CustomHttpHandler(
    GetHttpClient getHttpClient,
    LocalStorageService localStorageService,
    IUserAccountService userAccountService) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        bool login = request.RequestUri!.AbsoluteUri.Contains("login");
        bool register = request.RequestUri!.AbsoluteUri.Contains("register");
        bool refreshToken = request.RequestUri!.AbsoluteUri.Contains("refresh-token");

        if (login || register || refreshToken)
            return await base.SendAsync(request, cancellationToken);

        var result = await base.SendAsync(request, cancellationToken);
        if (result.StatusCode == HttpStatusCode.Unauthorized)
        {
            var stringToken = await localStorageService.GetToken();
            if (stringToken == null) return result;

            string token = String.Empty;
            try
            {
                token = request.Headers.Authorization!.Parameter!;
            }
            catch
            {
                // ignored
            }

            var deserializationToken = Serializations.DeserializeObj<UserSession>(stringToken);
            if (deserializationToken is null) return result;

            if (string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", deserializationToken.Token);
                return await base.SendAsync(request, cancellationToken);
            }

            var newToken = await GetRefreshToken(deserializationToken.RefreshToken);
            if (string.IsNullOrWhiteSpace(newToken)) return result;

            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", newToken);
            return await base.SendAsync(request, cancellationToken);
        }

        return result;
    }

    private async Task<string> GetRefreshToken(string? refreshToken)
    {
        var result = await userAccountService.RefreshTokenAsync(new RefreshToken() { Token = refreshToken });
        string serializedToken = Serializations.SerializeObj(new UserSession()
        {
            Token = result.Token,
            RefreshToken = result.RefreshToken
        });
        await localStorageService.SetToken(serializedToken);
        return result.Token;
    }
}