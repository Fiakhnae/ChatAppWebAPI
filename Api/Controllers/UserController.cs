using Application.Users;
using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("users")]
[Authorize]
public class UserController(ISender sender) : ControllerBase
{
    [HttpGet("me")]
    public async Task<IActionResult> GetAsync(CancellationToken cancellationToken = default)
    {
        var command = new GetProfileQuery();

        var response = await sender.Send(command, cancellationToken);

        return Ok(response);
    }
}
