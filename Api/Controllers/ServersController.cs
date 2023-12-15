using Api.Data;
using Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ServersController(ApplicationDbContext context) : ControllerBase
    {
        private readonly ApplicationDbContext _context = context;

        // GET: api/Servers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Server>>> GetServerList()
        {
            var currentUser = await _context.Users.FindAsync(User.Identity!.Name);

            if (currentUser == null)
            {
                return Unauthorized();
            }

            var server = _context.Servers.Where(x => x.Members.Any(x => x.UserId == currentUser.Id)).OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

            foreach (var s in server.Result)
            {
                s.Channels = await _context.Channels.Where(x => x.ServerId.Equals(s.Id)).ToListAsync(); ;
                s.Members = await _context.Members.Where(x => x.ServerId.Equals(s.Id)).ToListAsync();
            }

            return Ok(await server);
        }

        // GET: api/Servers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Server>> GetServer(string id)
        {
            var currentUser = await _context.Users.FindAsync(User.Identity!.Name);

            if (currentUser == null)
            {
                return Unauthorized();
            }

            var serverFromDb = await _context.Servers.FindAsync(Guid.Parse(id));

            if (serverFromDb == null)
            {
                return NotFound();
            }

            var member = await _context.Members.FirstOrDefaultAsync(x => x.UserId == currentUser.Id && x.ServerId == serverFromDb.Id);

            if (member == null)
            {
                return Unauthorized();
            }

            if (serverFromDb == null)
            {
                return NotFound();
            }

            if (currentUser == null)
            {
                return Unauthorized();
            }

            serverFromDb.Channels = await _context.Channels.Where(x => x.ServerId.Equals(serverFromDb.Id)).ToListAsync(); ;
            serverFromDb.Members = await _context.Members.Where(x => x.ServerId.Equals(serverFromDb.Id)).ToListAsync();


            return serverFromDb;
        }

        // Patch: api/Servers/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchServer(string id, Server server)
        {
            var currentUser = await _context.Users.FindAsync(User.Identity!.Name);

            if (currentUser == null)
            {
                return Unauthorized();
            }

            var serverFromDb = await _context.Servers.FindAsync(Guid.Parse(id));

            if (serverFromDb == null)
            {
                return NotFound();
            }

            if (serverFromDb.UserId != currentUser.Id)
            {
                // Check if the user is an admin of the server
                var member = await _context.Members.FirstOrDefaultAsync(x => x.UserId == currentUser.Id && x.ServerId == serverFromDb.Id);

                if (member == null || member.MemberRole != Member.MemberRoles.Admin)
                {
                    return Unauthorized();
                }
            }

            if (server.Name != serverFromDb.Name)
            {
                serverFromDb.Name = server.Name;
            }

            if (server.ImageUrl != serverFromDb.ImageUrl)
            {
                serverFromDb.ImageUrl = server.ImageUrl;
            }

            serverFromDb.UpdatedAt = DateTime.UtcNow;

            _context.Entry(serverFromDb).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ServerExists(Guid.Parse(id)))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Servers
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Server>> PostServer(Server server)
        {
            var currentUser = await _context.Users.FirstAsync(x => x.Id == User.Identity!.Name);

            if (currentUser == null)
            {
                return Unauthorized();
            }

            server.Id = Guid.NewGuid();
            server.User = currentUser;
            server.UserId = currentUser.Id;

            Member member = new()
            {
                User = currentUser,
                UserId = currentUser.Id,
                Username = currentUser.Username,
                ImageUrl = currentUser.ImageUrl,
                MemberRole = Member.MemberRoles.Admin,
                ServerId = server.Id,
                Server = server,
            };

            server.Members = new List<Member> { member };

            Channel channel = new()
            {
                Id = Guid.NewGuid(),
                Name = "general",
                Members = new List<Member> { member },
                ServerId = server.Id,
                Server = server,
                User = currentUser,
                UserId = currentUser.Id,
            };

            server.Channels = new List<Channel> { channel };

            _context.Servers.Add(server);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetServer", new
            {
                id = server.Id
            }, server);
        }

        // DELETE: api/Servers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteServer(string id)
        {
            var currentUser = await _context.Users.FindAsync(User.Identity!.Name);

            if (currentUser == null)
            {
                return Unauthorized();
            }

            var serverFromDb = await _context.Servers.FindAsync(Guid.Parse(id));

            if (serverFromDb == null)
            {
                return NotFound();
            }

            if (serverFromDb.UserId != currentUser.Id)
            {
                return Unauthorized();
            }

            _context.Servers.Remove(serverFromDb);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // PATCH: api/Servers/5/invite
        [HttpPatch("{id}/invite")]
        public async Task<IActionResult> PatchServerInvite(string id)
        {
            var currentUser = await _context.Users.FindAsync(User.Identity!.Name);

            if (currentUser == null)
            {
                return Unauthorized();
            }

            var serverFromDb = await _context.Servers.FindAsync(Guid.Parse(id));
            if (serverFromDb == null)
            {
                return NotFound();
            }
            // check if user's id matches server's user id
            if (serverFromDb.UserId != currentUser.Id)
            {
                var memberIsAdmin = await _context.Members.FirstOrDefaultAsync(x => x.UserId == currentUser.Id && x.ServerId == serverFromDb.Id && x.MemberRole == Member.MemberRoles.Admin);

                if (memberIsAdmin == null)
                {
                    return Unauthorized();
                }
            }

            serverFromDb.InviteCode = Guid.NewGuid().ToString();

            _context.Entry(serverFromDb).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ServerExists(serverFromDb.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(serverFromDb);
        }

        private bool ServerExists(Guid id)
        {
            return _context.Servers.Any(e => e.Id == id);
        }
    }
}
