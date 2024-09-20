using BaseLibrary.Dtos;
using Microsoft.AspNetCore.Mvc;
using ServerLibrary.Repositories.Contracts;

namespace Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController(IUserAccount repository) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] Register request)
    {
        if (request == null) return BadRequest("Model is null");
        var result = await repository.CreateAsync(request);

        return Ok(result);
    }
}