using Api.Data;
using Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ServersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Servers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Server>>> GetServerList()
        {
            var currentUser = await _context.Users.FindAsync(User.Identity!.Name);

            if (currentUser == null)
            {
                return Unauthorized();
            }

            //var servers = (from member in _context.Members
            //               where member.UserId == currentUser!.Id
            //               select member.ServerId).ToListAsync();

            var server = _context.Members
                .Where(x => x.UserId == currentUser.Id)
                .Select(x => x.ServerId)
                .ToListAsync();

            //await servers;

            return Ok(await server);
            //return servers;
            //return await _context.ServerList.ToListAsync();
        }

        // GET: api/Servers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Server>> GetServer(Guid id)
        {
            var currentUser = await _context.Members.FindAsync(User.Identity!.Name);
            var server = await _context.Servers.FindAsync(id);

            if (server == null)
            {
                return NotFound();
            }

            if (currentUser == null)
            {
                return Unauthorized();
            }

            return server;
        }

        // PUT: api/Servers/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutServer(Guid id, Server server)
        {
            if (id != server.Id)
            {
                return BadRequest();
            }

            _context.Entry(server).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ServerExists(id))
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
            if (User.Identity == null)
            {
                return Unauthorized();
            }

            var currentUser = await _context.Users.FirstAsync(x => x.Id == User.Identity.Name);

            if (currentUser == null)
            {
                return Unauthorized();
            }

            server.Id = Guid.NewGuid();
            server.User = currentUser;
            server.UserId = currentUser.Id;

            var newMember = new Member
            {
                User = currentUser,
                UserId = currentUser.Id,
                MemberRole = Member.MemberRoles.Admin,
                ServerId = server.Id,
                Server = server,
            };

            server.Members = new List<Member> { newMember };

            var newChannel = new Channel
            {
                Id = Guid.NewGuid(),
                Name = "general",
                Members = server.Members,
                ServerId = server.Id,
                Server = server
            };

            server.Channels = new List<Channel> { newChannel };

            currentUser.Servers.ToList().Add(server);
            currentUser.Channels.ToList().Add(newChannel);

            _context.Users.Update(currentUser);
            _context.Servers.Add(server);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetServer", new { id = server.Id }, server);
        }

        // DELETE: api/Servers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteServer(Guid id)
        {
            var server = await _context.Servers.FindAsync(id);
            if (server == null)
            {
                return NotFound();
            }

            _context.Servers.Remove(server);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ServerExists(Guid id)
        {
            return _context.Servers.Any(e => e.Id == id);
        }
    }
}
