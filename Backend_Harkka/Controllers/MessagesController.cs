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
        /// Get 20 public public messages orderded by post time
        /// </summary>
        /// <param name="page"></param>
        /// <returns>All messages</returns>
        [HttpGet("p_{page}")]

        public async Task<ActionResult<IEnumerable<MessageDTO>>> GetMessages(int page)
        {
            return Ok(await _messageService.GetMessagesAsync(page));
        }

        // GET: api/Messages/user/sent/p_page
        /// <summary>
        /// Get 20 messages sent by specified user ordered by post time
        /// </summary>
        /// <param name="username"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        [HttpGet("{username}/sent/p_{page}")]
        [Authorize]

        public async Task<ActionResult<IEnumerable<MessageDTO>>> GetMySentMessages(string username, int page)
        {
            if (username != this.User.FindFirst(ClaimTypes.Name)?.Value)
            {
                return Forbid();
            }
            return Ok(await _messageService.GetMySentMessagesAsync(username, page));
        }

        // GET: api/Messages/user/received/p_page
        /// <summary>
        /// Get 20 messages received by specified user ordered by post time, does not include public messages
        /// </summary>
        /// <param name="username"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        [HttpGet("{username}/received/p_{page}")]
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
        /// <param name="id"></param>
        /// <returns>Message specified by id or empty</returns>
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<MessageDTO>> GetMessage(long id)
        {
            string userName = this.User.FindFirst(ClaimTypes.Name)?.Value;
            if (!await _userAuthenticationService.isMyMessage(userName, id))
            {
                return Forbid();
            }
            MessageDTO message = await _messageService.GetMessageAsync(id);

            if (message == null)
            {
                return NotFound();
            }

            return Ok(message);
        }

        // PUT: api/Messages/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        /// <summary>
        /// Update message by id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutMessage(long id, MessageDTO message)
        {
            string userName = this.User.FindFirst(ClaimTypes.Name).Value;
            if (this.User.FindFirst(ClaimTypes.Name).Value != message.Sender)
            {
                return Forbid();
            }

            if (id != message.Id)
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
        /// Create new message
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<MessageDTO>> PostMessage(MessageDTO message)
        {
            MessageDTO? newMessage = await _messageService.NewMessageAsync(message);

            string userName = this.User.FindFirst(ClaimTypes.Name).Value;
            if (this.User.FindFirst(ClaimTypes.Name).Value != message.Sender)
            {
                return Forbid();
            }
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
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}/HardDelete")]
        [Authorize]
        public async Task<IActionResult> DeleteMessage(long id)
        {
            string userName = this.User.FindFirst(ClaimTypes.Name).Value;
            if (!await _userAuthenticationService.isMyMessage(userName, id))
            {
                return Forbid();
            }
            bool result = await _messageService.DeleteMessageAsync(id);
            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }

        // DELETE: api/Messages/5
        /// <summary>
        /// Soft Delete message specified by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> SoftDelete(long id)
        {
            string userName = this.User.FindFirst(ClaimTypes.Name).Value;
            if (!await _userAuthenticationService.isMyMessage(userName, id))
            {
                return Forbid();
            }
            bool result = await _messageService.SoftDeleteMessageAsync(id);
            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
