using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using BaseLibrary.Dtos;
using Microsoft.AspNetCore.Components.Authorization;

namespace ClientLibrary.Helpers;

public class CustomAuthenticationStateProvider(LocalStorageService localStorageService) : AuthenticationStateProvider
{
    private readonly ClaimsPrincipal anonymous = new(new ClaimsIdentity());

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var stringToken = await localStorageService.GetToken();
        if (string.IsNullOrWhiteSpace(stringToken)) return await Task.FromResult(new AuthenticationState(anonymous));

        var token = Serializations.DeserializeObj<UserSession>(stringToken);
        if (token == null) return await Task.FromResult(new AuthenticationState(anonymous));

        var getUserClaims = DecryptToken(token.Token);
        var claimsPrincipal = SetClaimsPrincipal(getUserClaims);
        return await Task.FromResult(new AuthenticationState(claimsPrincipal));
    }

    public async Task UpdateAuthenticationState(UserSession userSession)
    {
        var claimsPrincipal = new ClaimsPrincipal();
        if (userSession.Token != null || userSession.RefreshToken != null)
        {
            var serializeSession = Serializations.SerializeObj(userSession);
            await localStorageService.SetToken(serializeSession);
            var getUserClaims = DecryptToken(userSession.Token);
            claimsPrincipal = SetClaimsPrincipal(getUserClaims);
        }
        else
        {
            await localStorageService.RemoveToken();
        }
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(claimsPrincipal)));
    }

    private ClaimsPrincipal SetClaimsPrincipal(CustomUserClaims claims)
    {
        return new ClaimsPrincipal(new ClaimsIdentity(
            new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, claims.Id),
                new(ClaimTypes.Name, claims.Name),
                new(ClaimTypes.Role, claims.Role)
            },
            "JwtAuth"));
    }

    private CustomUserClaims DecryptToken(string? jwtToken)
    {
        if (string.IsNullOrWhiteSpace(jwtToken)) return new CustomUserClaims();

        var handler = new JwtSecurityTokenHandler();

        var token = handler.ReadJwtToken(jwtToken);
        var userId = token.Claims.First(x => x.Type == ClaimTypes.NameIdentifier);
        var name = token.Claims.First(x => x.Type == ClaimTypes.Name);
        var role = token.Claims.First(x => x.Type == ClaimTypes.Role);

        return new CustomUserClaims(userId.Value, name.Value, role.Value);
    }
}