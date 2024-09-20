using BaseLibrary.Dtos;
using BaseLibrary.Entities;
using BaseLibrary.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
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
        throw new NotImplementedException();
    }

    private async Task<ApplicationUser?> FindUserAsync(string userName) =>
        await dbContext.Users.FirstOrDefaultAsync(x => x.UserName.ToLower().Equals(userName.ToLower()));
}