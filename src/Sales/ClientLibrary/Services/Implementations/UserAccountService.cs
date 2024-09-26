using System.Net.Http.Json;
using BaseLibrary.Dtos;
using BaseLibrary.Responses;
using ClientLibrary.Helpers;
using ClientLibrary.Services.Contracts;

namespace ClientLibrary.Services.Implementations;

public class UserAccountService(GetHttpClient getHttpClient) : IUserAccountService
{
    public const string AuthUri = "api/authentication";

    public async Task<GeneralResponse> CreateAsync(Register user)
    {
        var httpClient = getHttpClient.GetPublicHttpClient();
        var result = await httpClient.PostAsJsonAsync($"{AuthUri}/register", user);
        if (!result.IsSuccessStatusCode) return new GeneralResponse(false, "Error occured");

        return (await result.Content.ReadFromJsonAsync<GeneralResponse>())!;
    }

    public async Task<LoginResponse> SignInAsync(Login user)
    {
        var httpClient = getHttpClient.GetPublicHttpClient();
        var result = await httpClient.PostAsJsonAsync($"{AuthUri}/login", user);

        if (!result.IsSuccessStatusCode) return new LoginResponse(false, "Error occured");

        return (await result.Content.ReadFromJsonAsync<LoginResponse>())!;
    }

    public async Task<LoginResponse> RefreshTokenAsync(RefreshToken token)
    {
        var httpClient = getHttpClient.GetPublicHttpClient();
        var result = await httpClient.PostAsJsonAsync($"{AuthUri}/refresh-token", token);

        if (!result.IsSuccessStatusCode) return new LoginResponse(false, "Error occured");

        return (await result.Content.ReadFromJsonAsync<LoginResponse>())!;
    }
}