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

namespace Backend_Harkka.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService service)
        {
            _userService = service;
        }

        // GET: api/Users
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetUsers()
        {
            return Ok(await _userService.GetUsersAsync());
        }

        // GET: api/Users/username
        [HttpGet("{username}")]
        public async Task<ActionResult<UserDTO>> GetUser(string username)
        {
            UserDTO? user = await _userService.GetUserAsync(username);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        // PUT: api/Users/username
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{username}")]
        public async Task<IActionResult> PutUser(string username, User user)
        {
            if (username != user.UserName)
            {
                return BadRequest();
            }

            if (await _userService.UpdateUserAsync(user))
            {
                return NoContent();
            }

            return NotFound();
        }

        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<UserDTO>> PostUser(User user)
        {
            UserDTO? newUser = await _userService.NewUserAsync(user);
            if (newUser == null)
            {
                return Problem("Username not available", statusCode:400);
            }

            return CreatedAtAction("GetUser", new {username = user.UserName}, user);
        }

        // DELETE: api/Users/username
        [HttpDelete("{username}")]
        public async Task<IActionResult> DeleteUser(string username)
        {
            bool result = await _userService.DeleteUserAsync(username);
            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }

    }
}
