using Api.Data;
using Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _config;



        public UsersController(ApplicationDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            var currentUser = await _context.Users.FindAsync(User.Identity!.Name);


            if (currentUser == null)
            {
                return Unauthorized();
            }

            return Ok(await _context.Users.ToListAsync());
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(string id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        // PATCH: api/Users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPatch("{id}")]
        public async Task<IActionResult> PutUser(string id, User user)
        {
            if (id != User.Identity!.Name)
            {
                return Unauthorized();
            }

            var currentUser = await _context.Users.FindAsync(User.Identity!.Name);

            if (currentUser == null)
            {
                return Unauthorized();
            }

            if (currentUser.Id != user.Id)
            {
                return Unauthorized();
            }

            // TODO: Allow users to change their username, email, and other fields
            if (currentUser.ImageUrl != user.ImageUrl)
            {
                currentUser.ImageUrl = user.ImageUrl;
            }

            _context.Entry(currentUser).State = EntityState.Modified;

            var updateAuth0User = await UpdateAuth0User(currentUser);
            if (!updateAuth0User)
            {
                return BadRequest();
            }

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(UserDto user)
        {
            // null checks for userDto
            if (user.Id == null || user.Username == null || user.ImageUrl == null)
            {
                return BadRequest();
            }

            // create a new user from the userDto
            var newUser = new User
            {
                Id = user.Id,
                Username = user.Username,
                ImageUrl = user.ImageUrl,
                Channels = [],
                Members = [],
                Servers = []
            };

            _context.Users.Add(newUser);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (UserExists(user.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetUser", new { id = user.Id }, user);
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var requestUser = User.Identity?.Name;
            if (requestUser == null || requestUser != id)
            {
                return Unauthorized();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            await DeleteAuth0User(user);

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserExists(string id)
        {
            return _context.Users.Any(e => e.Id == id);
        }

        // TODO: Move this to a service
        private async Task<bool> UpdateAuth0User(User user)
        {
            var client = new HttpClient();
            var url = $"https://{_config["Auth0:Domain"]}/api/v2/users/{user.Id}";
            var request = new HttpRequestMessage(HttpMethod.Patch, url);
            request.Headers.Add("Accept", "application/json");
            request.Headers.Add("Authorization", $"Bearer {_config["Auth0:ManagementToken"]}");
            var content = new StringContent("{\"picture\":\"" + user.ImageUrl + "\"}", null, "application/json");
            request.Content = content;
            var response = await client.SendAsync(request);
            return response.IsSuccessStatusCode;
        }

        private async Task<bool> DeleteAuth0User(User user)
        {
            var client = new HttpClient();
            var url = $"https://{_config["Auth0:Domain"]}/api/v2/users/{user.Id}";
            var request = new HttpRequestMessage(HttpMethod.Delete, url);
            request.Headers.Add("Authorization", $"Bearer {_config["Auth0:ManagementToken"]}");
            var response = await client.SendAsync(request);
            return response.IsSuccessStatusCode;
        }
    }
}
