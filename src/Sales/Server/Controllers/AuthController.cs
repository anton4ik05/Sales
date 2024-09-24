using BaseLibrary.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using ServerLibrary.Repositories.Contracts;

namespace Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController(IUserAccount repository) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> CreateAsync([BindRequired] [FromBody] Register user)
    {
        var result = await repository.CreateAsync(user);

        return Ok(result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> SignInAsync([BindRequired] [FromBody] Login user)
    {
        var result = await repository.SignInAsync(user);

        return Ok(result);
    }
}