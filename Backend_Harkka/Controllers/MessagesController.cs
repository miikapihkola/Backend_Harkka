using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend_Harkka.Models;
using Backend_Harkka.Services;

namespace Backend_Harkka.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly IMessageService _messageService;

        public MessagesController(IMessageService service)
        {
            _messageService = service;
        }

        // GET: api/Messages
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MessageDTO>>> GetMessages()
        {
            return Ok(await _messageService.GetMessagesAsync());
        }

        // GET: api/Messages/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MessageDTO>> GetMessage(long id)
        {
            MessageDTO message = await _messageService.GetMessageAsync(id);

            if (message == null)
            {
                return NotFound();
            }

            return Ok(message);
        }

        // PUT: api/Messages/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMessage(long id, MessageDTO message)
        {
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
        [HttpPost]
        public async Task<ActionResult<MessageDTO>> PostMessage(MessageDTO message)
        {
            MessageDTO? newMessage = await _messageService.NewMessageAsync(message);

            if (newMessage == null)
            {
                return Problem();
            }

            return CreatedAtAction("GetMessage", new { id = message.Id }, message);
        }

        // DELETE: api/Messages/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMessage(long id)
        {
            bool result = await _messageService.DeleteMessageAsync(id);
            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }

    }
}
