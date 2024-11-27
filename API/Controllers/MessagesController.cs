using System;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Authorize]
public class MessagesController(IMessageRepository messageRepository, IUserRepository userRepository, IMapper mapper) : BaseApiController
{
    [HttpPost]
    public async Task<ActionResult<MessageDto>> CreateMessage(CreateMessageDto createMessageDto)
    {
        var userName = User.GetUsername();

        if (userName == createMessageDto.RecipientUsername.ToLower())
            return BadRequest("You cannot send message to yourself");

        var sender = await userRepository.GetUserByUsernameAsync(userName);
        var recipient = await userRepository.GetUserByUsernameAsync(createMessageDto.RecipientUsername.ToLower());

        if (sender == null || recipient == null || sender.UserName == null || recipient.UserName == null)
            return BadRequest("cannot sent the message aat the moment");

        var message = new Message
        {
            Sender = sender,
            Recipient = recipient,
            Content = createMessageDto.Content,
            SenderUsername = sender.UserName,
            RecipientUsername = recipient.UserName
        };

        messageRepository.AddMessage(message);
        if (await messageRepository.SaveAllAsync()) return Ok(mapper.Map<MessageDto>(message));

        return BadRequest("Failed to save the message");
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessagesForUser([FromQuery] MessageParams messageParams)
    {
        messageParams.Username = User.GetUsername();

        var messages = await messageRepository.GetMessagesForUser(messageParams);

        Response.AddPaginationHeaders(messages);

        return messages;
    }

    [HttpGet("thread/{username}")]
    public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessageThread(string username)
    {
        var currentUsername = User.GetUsername();

        return Ok(await messageRepository.GetMessageThread(currentUsername, username));
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteMessage(int id)
    {
        var username = User.GetUsername();

        var message = await messageRepository.GetMessage(id);

        if (message == null) return BadRequest("Cannot delete this message. Message not found");

        if (message.SenderUsername != username && message.RecipientUsername != username) return Forbid();

        if (message.SenderUsername == username) message.SenderDeleted = true;
        if (message.RecipientUsername == username) message.RecipientDeleted = true;

        if (message is { SenderDeleted: true, RecipientDeleted: true })
        {
            messageRepository.DeleteMessage(message);
        }

        if (await messageRepository.SaveAllAsync()) return Ok();

        return BadRequest("Problem in deleting this message");
    }
}
