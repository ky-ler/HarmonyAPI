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

            var server = await _context.Servers.FirstOrDefaultAsync(x => x.Id.ToString() == id);

            if (server == null)
            {
                return NotFound();
            }

            var member = await _context.Members.FirstOrDefaultAsync(x => x.UserId == currentUser.Id && x.ServerId == server.Id);

            if (member == null)
            {
                return Unauthorized();
            }

            if (server == null)
            {
                return NotFound();
            }

            if (currentUser == null)
            {
                return Unauthorized();
            }

            server.Channels = await _context.Channels.Where(x => x.ServerId.Equals(server.Id)).ToListAsync(); ;
            server.Members = await _context.Members.Where(x => x.ServerId.Equals(server.Id)).ToListAsync();


            return server;
        }

        // PUT: api/Servers/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutServer(Server server)
        // Server contains Guid id - not needed
        //public async Task<IActionResult> PutServer(Guid id, Server server)
        {
            var currentUser = await _context.Users.FindAsync(User.Identity!.Name);

            var serverFromDb = await _context.Servers.FindAsync(server.Id);

            if (serverFromDb == null)
            {
                return NotFound();
            }

            if (currentUser == null)
            {
                return Unauthorized();
            }

            // Check if the user is an admin of the server
            var member = await _context.Members.FirstOrDefaultAsync(x => x.UserId == currentUser.Id && x.ServerId == serverFromDb.Id);

            if (member == null || member.MemberRole != Member.MemberRoles.Admin)
            {
                return Unauthorized();
            }

            //if (id != server.Id)
            //{
            //return BadRequest();
            //}

            _context.Entry(server).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ServerExists(server.Id))
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
        public async Task<IActionResult> DeleteServer(Guid id)
        {
            var currentUser = await _context.Users.FindAsync(User.Identity!.Name);

            if (currentUser == null)
            {
                return Unauthorized();
            }

            var server = await _context.Servers.FindAsync(id);
            if (server == null)
            {
                return NotFound();
            }

            if (server.UserId != currentUser.Id)
            {
                return Unauthorized();
            }

            _context.Servers.Remove(server);
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

            var server = await _context.Servers.FirstOrDefaultAsync(x => x.Id.ToString() == id);
            if (server == null)
            {
                return NotFound();
            }

            var memberIsAdmin = await _context.Members.FirstOrDefaultAsync(x => x.UserId == currentUser.Id && x.ServerId == server.Id && x.MemberRole == Member.MemberRoles.Admin);

            if (memberIsAdmin == null)
            {
                return Unauthorized();
            }

            server.InviteCode = Guid.NewGuid().ToString();

            _context.Entry(server).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ServerExists(server.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(server);
        }

        private bool ServerExists(Guid id)
        {
            return _context.Servers.Any(e => e.Id == id);
        }
    }
}
