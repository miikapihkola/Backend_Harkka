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

        // GET: api/Users/p_page
        /// <summary>
        /// Gets the information of 20 users in database ordered by username, increasing pagenumber shifts startpoint to later point
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        [HttpGet("p_{page}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetUsers(int page)
        {
            return Ok(await _userService.GetUsersAsync(page));
        }

        // GET: api/Users/username
        /// <summary>
        /// Gets user specified by username
        /// </summary>
        /// <param name="username"></param>
        /// <returns>User information for one user or empty</returns>
        [HttpGet("{username}")]
        [Authorize]
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
        /// <summary>
        /// Update user information
        /// </summary>
        /// <param name="username"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPut("{username}")]
        [Authorize]
        public async Task<IActionResult> PutUser(string username, User user)
        {
            if (username != this.User.FindFirst(ClaimTypes.Name).Value)
            {
                return Forbid();
            }
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
        /// <summary>
        /// Create new user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
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

        // DELETE: api/Users/username/HardDelete
        /// <summary>
        /// Delete user specified by username
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        [HttpDelete("{username}/HardDelete")]
        [Authorize]

        // Delete muuttaminen sellaiseksi että username muutetaan deleted ja poistetaan muut tietueet sekä kutsutaan message controllerin deleteä jokaseen viestiin

        public async Task<IActionResult> DeleteUser(string username)
        {
            if (username != this.User.FindFirst(ClaimTypes.Name).Value)
            {
                return Forbid();
            }
            bool result = await _userService.DeleteUserAsync(username);
            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }

        // DELETE: api/Users/username
        /// <summary>
        /// Wipe user specified by username
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        [HttpDelete("{username}")]
        [Authorize]
        public async Task<IActionResult> SoftDeleteUser(string username)
        {
            if (username != this.User.FindFirst(ClaimTypes.Name).Value)
            {
                return Forbid();
            }
            bool result = await _userService.SoftDeleteUserAsync(username);
            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }

    }
}
