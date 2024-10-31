using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend_Harkka.Models;
using Backend_Harkka.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Backend_Harkka.Middleware;
using Microsoft.AspNetCore.Identity;

namespace Backend_Harkka.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly IMessageService _messageService;
        private readonly IUserAuthenticationService _userAuthenticationService;

        public MessagesController(IMessageService service, IUserAuthenticationService authenticationService)
        {
            _messageService = service;
            _userAuthenticationService = authenticationService;
        }

        // GET: api/Messages/p_page
        /// <summary>
        /// Get 20 public public messages ordered by post time, increasing pagenumber shifts startpoint to later point, does not include messages marked as deleted
        /// </summary>
        /// <param name="page"></param>
        /// <returns>All messages</returns>
        [HttpGet("p_{page}")]

        public async Task<ActionResult<IEnumerable<MessageDTO>>> GetMessages(int page)
        {
            return Ok(await _messageService.GetMessagesAsync(page));
        }

        // GET: api/Messages/username/sent/p_page
        /// <summary>
        /// Get 20 messages sent by specified user ordered by post time, increasing pagenumber shifts startpoint to later point, does not include messages marked as deleted
        /// </summary>
        /// <param name="username"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        [HttpGet("{username}/Sent/p_{page}")]
        [Authorize]

        public async Task<ActionResult<IEnumerable<MessageDTO>>> GetMySentMessages(string username, int page)
        {
            if (username != this.User.FindFirst(ClaimTypes.Name)?.Value)
            {
                return Forbid();
            }
            return Ok(await _messageService.GetMySentMessagesAsync(username, page));
        }

        // GET: api/Messages/username/received/p_page
        /// <summary>
        /// Get 20 messages received by specified user ordered by post time, increasing pagenumber shifts startpoint to later point, does not include public messages or messages marked as deleted
        /// </summary>
        /// <param name="username"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        [HttpGet("{username}/Received/p_{page}")]
        [Authorize]

        public async Task<ActionResult<IEnumerable<MessageDTO>>> GetMyReceivedMessages(string username, int page)
        {
            if (username != this.User.FindFirst(ClaimTypes.Name)?.Value)
            {
                return Forbid();
            }
            return Ok(await _messageService.GetMyReceivedMessagesAsync(username, page));
        }

        // GET: api/Messages/5
        /// <summary>
        /// Get message by id
        /// </summary>
        /// <param name="messageId"></param>
        /// <returns>Message specified by id or empty</returns>
        [HttpGet("{messageId}")]
        [Authorize]
        public async Task<ActionResult<MessageDTO>> GetMessage(long messageId)
        {
            string userName = this.User.FindFirst(ClaimTypes.Name)?.Value;
            if (!await _userAuthenticationService.IsMyMessage(userName, messageId))
            {
                return Forbid();
            }
            MessageDTO message = await _messageService.GetMessageAsync(messageId);

            if (message == null)
            {
                return NotFound();
            }

            return Ok(message);
        }

        // PUT: api/Messages/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        /// <summary>
        /// Update message by id, updates EditTime
        /// </summary>
        /// <param name="messageId"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        [HttpPut("{messageId}")]
        [Authorize]
        public async Task<IActionResult> PutMessage(long messageId, MessageDTO message)
        {
            string userName = this.User.FindFirst(ClaimTypes.Name).Value;
            if (this.User.FindFirst(ClaimTypes.Name).Value != message.Sender)
            {
                return Forbid();
            }

            if (messageId != message.Id)
            {
                return BadRequest();
            }

            bool result = await _messageService.UpdateMessageAsync(message);

            if (!result)
            {
                return NotFound();
            }


            return NoContent();
        }

        // POST: api/Messages
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        /// <summary>
        /// Create new message, increase senders sent messages and recipients(if included) received messages number by 1 
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<MessageDTO>> PostMessage(MessageDTO message)
        {
            string userName = this.User.FindFirst(ClaimTypes.Name).Value;
            if (this.User.FindFirst(ClaimTypes.Name).Value != message.Sender)
            {
                return Forbid();
            }
            MessageDTO? newMessage = await _messageService.NewMessageAsync(message);
            if (newMessage == null)
            {
                return Problem();
            }

            return CreatedAtAction("GetMessage", new { id = newMessage.Id }, newMessage);
        }

        // DELETE: api/Messages/5/HardDelete
        /// <summary>
        /// Delete message specified by id, Can break message threads
        /// </summary>
        /// <param name="messageId"></param>
        /// <returns></returns>
        [HttpDelete("{messageId}/HardDelete")]
        [Authorize]
        public async Task<IActionResult> DeleteMessage(long messageId)
        {
            string userName = this.User.FindFirst(ClaimTypes.Name).Value;
            if (!await _userAuthenticationService.IsMyMessage(userName, messageId))
            {
                return Forbid();
            }
            bool result = await _messageService.DeleteMessageAsync(messageId);
            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }

        // DELETE: api/Messages/5
        /// <summary>
        /// Wipe message specified by id (Changes title and body as "Deleted Message", changes IsDeleted value as True), updates EditTime
        /// </summary>
        /// <param name="messageId"></param>
        /// <returns></returns>
        [HttpDelete("{messageId}")]
        [Authorize]
        public async Task<IActionResult> SoftDelete(long messageId)
        {
            string userName = this.User.FindFirst(ClaimTypes.Name).Value;
            if (!await _userAuthenticationService.IsMyMessage(userName, messageId))
            {
                return Forbid();
            }
            bool result = await _messageService.SoftDeleteMessageAsync(messageId);
            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
