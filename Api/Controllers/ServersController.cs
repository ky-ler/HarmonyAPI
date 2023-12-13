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

            var server = _context.Servers.Where(x => x.Members.Any(x => x.UserId == currentUser.Id)).OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

            //var messages = await _context.Messages.Where(x => x.Channel.ServerId.Equals(server.Id)).ToListAsync();

            foreach (var s in server.Result)
            {
                s.Channels = await _context.Channels.Where(x => x.ServerId.Equals(s.Id)).ToListAsync(); ;
                s.Members = await _context.Members.Where(x => x.ServerId.Equals(s.Id)).ToListAsync();


                //foreach (var c in s.Channels)
                //{
                //c.Members = await _context.Members.Where(x => x.ServerId.Equals(c.ServerId)).ToListAsync();

                //c.Messages = await _context.Messages.Where(x => x.Channel.ServerId.Equals(c.ServerId)).ToListAsync(); ;
                //}
            }

            return Ok(await server);
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

            //server.Members = new List<Member> {
            //    new() {
            //        User = currentUser,
            //        UserId = currentUser.Id,
            //        MemberRole = Member.MemberRoles.Admin,
            //        ServerId = server.Id,
            //        Server = server,
            //    }
            //};

            //server.Members = new List<Member> { newMember };
            Member member = new()
            {
                User = currentUser,
                UserId = currentUser.Id,
                MemberRole = Member.MemberRoles.Admin,
                ServerId = server.Id,
                Server = server,
            };

            //_context.Members.Add(member);

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
            //_context.Channels.Add(channel);

            server.Channels = new List<Channel> { channel };

            //currentUser.Servers.ToList().Add(server);
            //currentUser.Channels.ToList().Add(newChannel);

            //_context.Users.Update(currentUser);
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
