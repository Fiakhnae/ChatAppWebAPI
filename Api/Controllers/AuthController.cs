using Application.Abstractions;
using Application.Auth;
using Mediator;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

public record SignUpRequest(string Email, string Username, string Password, int Gender, DateOnly? BirthDate, bool RememberMe);

public record SignInRequest(string UsernameOrEmail, string Password, bool RememberMe);



[ApiController]
[Route("auth")]
public class AuthController(
    ISender sender,
    IClaimsPrincipalProvider claimsPrincipalProvider) : ControllerBase
{
    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> MeAsync(CancellationToken cancellationToken = default)
    {
        var command = new MeCommand();

        var response = await sender.Send(command, cancellationToken);

        return Ok(response);
    }

    [HttpPost("sign-up")]
    public async Task<IActionResult> SignUpAsync(SignUpRequest request, CancellationToken cancellationToken = default)
    {
        var command = new SignUpCommand(request.Email, request.Username, request.Password, request.Gender, request.BirthDate);

        var response = await sender.Send(command, cancellationToken);

        var principal = claimsPrincipalProvider.Create(response.Id, response.Username, response.Email, response.SecurityStamp);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal,
            new AuthenticationProperties
            {
                IsPersistent = true,
                AllowRefresh = true,
                ExpiresUtc = request.RememberMe ? DateTimeOffset.UtcNow.AddDays(7) : DateTimeOffset.UtcNow.AddHours(1)
            });

        return Ok();
    }

    [HttpPost("sign-in")]
    public async Task<IActionResult> SignInAsync(SignInRequest request, CancellationToken cancellationToken = default)
    {
        var command = new SignInCommand(request.UsernameOrEmail, request.Password);

        var response = await sender.Send(command, cancellationToken);

        var principal = claimsPrincipalProvider.Create(response.Id, response.Username, response.Email, response.SecurityStamp);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal,
            new AuthenticationProperties
            {
                IsPersistent = true,
                AllowRefresh = true,
                ExpiresUtc = request.RememberMe ? DateTimeOffset.UtcNow.AddDays(7) : DateTimeOffset.UtcNow.AddHours(1)
            });

        return Ok();
    }

    [Authorize]
    [HttpPost("sign-out")]
    public async Task<IActionResult> SignOutAsync(CancellationToken cancellationToken = default)
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        return Ok();
    }

    [Authorize]
    [HttpPost("sign-out-all")]
    public async Task<IActionResult> SignOutAllAsync(CancellationToken cancellationToken = default)
    {
        var command = new SignOutAllCommand();

        await sender.Send(command, cancellationToken);

        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        return Ok();
    }
}
