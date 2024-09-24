using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using BaseLibrary.Dtos;
using BaseLibrary.Entities;
using BaseLibrary.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ServerLibrary.Data;
using ServerLibrary.Helpers;
using ServerLibrary.Repositories.Contracts;

namespace ServerLibrary.Repositories.Implementations;

public class UserAccountRepository(IOptions<JwtSection> config, AppDbContext dbContext) : IUserAccount
{
    public async Task<GeneralResponse> CreateAsync(Register? user)
    {
        if (user is null) return new GeneralResponse(false, "User is null");

        var checkUser = await FindUserAsync(user.UserName);
        if (checkUser != null) return new GeneralResponse(false, "User already exists");

        var appUser = await AddToDatabaseAsync(new ApplicationUser
        {
            UserName = user.UserName,
            Password = BCrypt.Net.BCrypt.HashPassword(user.Password),
        });

        var checkAdminRole = await dbContext.SystemRoles.FirstOrDefaultAsync(x => x.Name!.Equals("Admin"));
        if (checkAdminRole is null)
        {
            var createAdminRole = await AddToDatabaseAsync(new SystemRole() { Name = Constants.Admin });
            await AddToDatabaseAsync(new UserRole() { RoleId = createAdminRole.Id, UserId = appUser.Id });
            return new GeneralResponse(true, "Account created");
        }

        var checkUserRole = await dbContext.SystemRoles.FirstOrDefaultAsync(x => x.Name!.Equals(Constants.User));
        if (checkUserRole is null)
        {
            var response = await AddToDatabaseAsync(new SystemRole() { Name = Constants.User });
            await AddToDatabaseAsync(new UserRole() { RoleId = response.Id, UserId = appUser.Id });
        }
        else
        {
            await AddToDatabaseAsync(new UserRole() { RoleId = checkUserRole.Id, UserId = appUser.Id });
        }

        return new GeneralResponse(true, "Account created");
    }

    private async Task<T> AddToDatabaseAsync<T>(T model)
    {
        var result = dbContext.Add(model!);
        await dbContext.SaveChangesAsync();
        return (T)result.Entity;
    }

    public async Task<LoginResponse> SignInAsync(Login user)
    {
        var appUser = await FindUserAsync(user.UserName);
        if (appUser is null) return new LoginResponse(false, "User not found");

        if (!BCrypt.Net.BCrypt.Verify(user.Password, appUser.Password))
            return new LoginResponse(false, "Invalid password");

        var getUserRole = await FindUserRoleAsync(appUser.Id);
        if (getUserRole is null) return new LoginResponse(false, "User role not found");

        var getUserRoleName = await FindRoleNameAsync(getUserRole.RoleId);
        if (getUserRoleName is null) return new LoginResponse(false, "User role not found");

        string jwtToken = GenerateToken(appUser, getUserRoleName.Name);
        string refreshToken = GenerateRefreshToken();
        
        var findUser = await dbContext.RefreshTokenInfos.FirstOrDefaultAsync(x => x.UserId == appUser.Id);
        if (findUser is not null)
        {

            findUser.Token = refreshToken;
            await dbContext.SaveChangesAsync();
        }
        else
        {
            await AddToDatabaseAsync(new RefreshTokenInfo() { UserId = appUser.Id, Token = refreshToken });
        }
        return new LoginResponse(true, "Login successful", jwtToken, refreshToken);
    }

    public async Task<LoginResponse> RefreshTokenAsync(RefreshToken token)
    {
        var findToken = await dbContext.RefreshTokenInfos.FirstOrDefaultAsync(x => x.Token!.Equals(token.Token));
        if (findToken is null) return new LoginResponse(false, "Invalid token");

        var user = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == findToken.UserId);
        if (user is null) return new LoginResponse(false, "User not found");

        var userRole = await FindUserRoleAsync(user.Id);
        var roleName = await FindRoleNameAsync(userRole!.RoleId);
        string jwtToken = GenerateToken(user, roleName!.Name);
        string refreshToken = GenerateRefreshToken();

        var updateRefreshToken = await dbContext.RefreshTokenInfos.FirstOrDefaultAsync(x => x.UserId == user.Id);
        if (updateRefreshToken is null) return new LoginResponse(false, "Refresh token not found");

        updateRefreshToken.Token = refreshToken;
        await dbContext.SaveChangesAsync();
        return new LoginResponse(true, "Refresh token successful", jwtToken, refreshToken);
    }

    private static string GenerateRefreshToken() =>
        Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

    private string GenerateToken(ApplicationUser appUser, string? role)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.Value.Key!));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var userClaims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, appUser.Id.ToString()),
            new Claim(ClaimTypes.Name, appUser.UserName),
            new Claim(ClaimTypes.Role, role!),
        };

        var token = new JwtSecurityToken(
            issuer: config.Value.Issuer,
            audience: config.Value.Audience,
            signingCredentials: credentials,
            expires: DateTime.Now.AddDays(1),
            claims: userClaims
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private async Task<ApplicationUser?> FindUserAsync(string userName) =>
        await dbContext.Users.FirstOrDefaultAsync(x => x.UserName.ToLower().Equals(userName.ToLower()));

    private async Task<SystemRole?> FindRoleNameAsync(int roleId) =>
        await dbContext.SystemRoles.FirstOrDefaultAsync(x => x.Id == roleId);

    private async Task<UserRole?> FindUserRoleAsync(int userId) =>
        await dbContext.UserRoles.FirstOrDefaultAsync(x => x.UserId == userId);
}