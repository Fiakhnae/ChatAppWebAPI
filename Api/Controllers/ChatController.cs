using Api.Hubs;
using Application.Chats;
using Application.Messages;
using Application.Users;
using Domain.Entities;
using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Api.Controllers;

public record CreateChatRequest(string Name);

public record UpdateChatNameRequest(string NewName);

public record AddChatMemberRequest(string Username);

public record RemoveChatMemberRequest(Guid UserId);

public record SendMessageRequest(string Message);

[ApiController]
[Route("chats")]
[Authorize]
public class ChatController(
    ISender sender, IHubContext<UserHub> hub) : ControllerBase
{
    [HttpPost("{chatId}/members")]
    public async Task<IActionResult> AddMemberAsync(Guid chatId, AddChatMemberRequest request, CancellationToken cancellationToken = default)
    {
        var command = new AddMemberCommand(chatId, request.Username);

        await sender.Send(command, cancellationToken);

        return Created();
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync(CreateChatRequest request, CancellationToken cancellationToken = default)
    {
        var command = new CreateChatCommand(request.Name);

        await sender.Send(command, cancellationToken);

        return Created();
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetChatAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var query = new GetChatQuery(id);

        var response = await sender.Send(query, cancellationToken);

        return Ok(response);
    }

    [HttpGet]
    public async Task<IActionResult> GetChatsAsync(CancellationToken cancellationToken = default)
    {
        var query = new GetChatsQuery();

        var response = await sender.Send(query, cancellationToken);

        return Ok(response);
    }

    [HttpDelete("{chatId}/members")]
    public async Task<IActionResult> RemoveMemberAsync(Guid chatId, RemoveChatMemberRequest request, CancellationToken cancellationToken = default)
    {
        var command = new RemoveMemberCommand(chatId, request.UserId);

        await sender.Send(command, cancellationToken);

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteChatAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var command = new RemoveChatCommand(id);

        await sender.Send(command, cancellationToken);

        return NoContent();
    }

    [HttpPatch("{chatId}/update-name")]
    public async Task<IActionResult> UpdateNameAsync(Guid chatId, UpdateChatNameRequest request, CancellationToken cancellationToken = default)
    {
        var command = new UpdateChatNameCommand(chatId, request.NewName);

        await sender.Send(command, cancellationToken);

        return Ok();
    }

    [HttpPost("{chatId}/messages")]
    public async Task<IActionResult> SendMessage(Guid chatId, SendMessageRequest request, CancellationToken cancellationToken = default)
    {
        var command = new SendMessageCommand(chatId, request.Message);

        await sender.Send(command, cancellationToken);

        var newcommand = new GetProfileQuery();

        var newresponse = await sender.Send(newcommand, cancellationToken);

        var newquery = new GetChatQuery(chatId);

        var chatresponse = await sender.Send(newquery, cancellationToken);

        var query1 = new GetChatQuery(chatId);

        var response1 = await sender.Send(query1, cancellationToken);

        var users = response1.Members.Select(o => o.UserId);

        foreach (var id in users)
        {
            if (id != newresponse.Id)
            {
                await hub.Clients
                    .Group($"user-{id}")
                    .SendAsync("OnGroupAssigned", new
                    {
                        GroupName = $"{chatresponse.Name}",
                        ChatId = chatresponse.Id
                    });
            }
        }

        return Created();
    }

    [HttpGet("{chatId}/messages")]
    public async Task<IActionResult> GetMessages(Guid chatId, CancellationToken cancellationToken = default)
    {
        var query = new GetMessagesQuery(chatId);

        var response = await sender.Send(query, cancellationToken);

        return Ok(response);
    }
}
