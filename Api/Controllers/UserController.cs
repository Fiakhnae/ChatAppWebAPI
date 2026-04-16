using Api.Hubs;
using Application.Users;
using Domain.Entities;
using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Api.Controllers;

[ApiController]
[Route("users")]
[Authorize]
public class UserController(ISender sender, IHubContext<UserHub> hub) : ControllerBase
{
    [HttpGet("me")]
    public async Task<IActionResult> GetAsync(CancellationToken cancellationToken = default)
    {
        var command = new GetProfileQuery();

        var response = await sender.Send(command, cancellationToken);

        return Ok(response);
    }
}
