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
        public async Task<ActionResult<IEnumerable<ServerDto>>> GetServerList()
        {
            var currentUser = await _context.Users.FindAsync(User.Identity!.Name);

            if (currentUser == null)
            {
                return Unauthorized();
            }

            // select all servers where user is a member while preventing object cycles
            var s = await _context.Servers.Where(x => x.Members.Any(x => x.UserId == currentUser.Id)).Select(x => new
            {
                x.Id,
                x.Name,
                x.ImageUrl,
            }).ToListAsync();

            return Ok(s);
        }

        // GET: api/Servers/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetServer(string id)
        {
            var currentUser = await _context.Users.FindAsync(User.Identity!.Name);

            if (currentUser == null)
            {
                return Unauthorized();
            }

            //check is request user is a member of the server
            var currentUserMember = await _context.Members.FirstOrDefaultAsync(x => x.UserId == currentUser.Id && x.ServerId == Guid.Parse(id));
            if (currentUserMember == null)
            {
                return Unauthorized();
            }


            // select server and filter out object cycles
            var s = await _context.Servers.Where(x => x.Id == Guid.Parse(id)).Select(x => new
            {
                x.Id,
                x.Name,
                x.ImageUrl,
                //x.UserId,
                // include userId if user is admin
                UserId = currentUserMember.MemberRole != Member.MemberRoles.Member ? x.UserId : null,
                // include invite code if user is admin
                InviteCode = currentUserMember.MemberRole != Member.MemberRoles.Member ? x.InviteCode : null,
                Members = x.Members.OrderBy(x => x.MemberRole).Select(x => new
                {
                    x.Id,
                    //x.UserId, // not needed
                    x.User!.Username,
                    x.User!.ImageUrl,
                    x.MemberRole,
                }),
                Channels = x.Channels.Select(x => new
                {
                    x.Id,
                    x.Name,
                    x.ChannelType,
                }),
            }).FirstOrDefaultAsync();

            if (s == null)
            {
                return NotFound();
            }

            //serverFromDb.Channels = await _context.Channels.Where(x => x.ServerId.Equals(serverFromDb.Id)).ToListAsync(); ;
            //serverFromDb.Members = await _context.Members.Where(x => x.ServerId.Equals(serverFromDb.Id)).OrderBy(x => x.MemberRole).ToListAsync();


            return Ok(s);
        }

        // Patch: api/Servers/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchServer(string id, ServerDto server)
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

            if (server.Name != serverFromDb.Name && server.Name != null)
            {
                serverFromDb.Name = server.Name;
            }

            if (server.ImageUrl != serverFromDb.ImageUrl && server.ImageUrl != null)
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
        public async Task<ActionResult<ServerDto>> PostServer(ServerDto server)
        {
            // null check name and image url
            if (server.Name == null || server.ImageUrl == null)
            {
                return BadRequest();
            }

            var currentUser = await _context.Users.FirstAsync(x => x.Id == User.Identity!.Name);

            if (currentUser == null)
            {
                return Unauthorized();
            }

            // create a new server from the serverDto
            Server newServer = new()
            {
                Id = Guid.NewGuid(),
                User = currentUser,
                UserId = currentUser.Id,
                Name = server.Name,
                ImageUrl = server.ImageUrl,
                InviteCode = Guid.NewGuid().ToString(),
                Members = [],
                Channels = [],
            };

            Member member = new()
            {
                User = currentUser,
                UserId = currentUser.Id,
                MemberRole = Member.MemberRoles.Admin,
                ServerId = newServer.Id,
                Server = newServer,
            };

            newServer.Members.Add(member);

            Channel channel = new()
            {
                Id = Guid.NewGuid(),
                Name = "general",
                ServerId = newServer.Id,
                Server = newServer,
                ChannelType = Channel.ChannelTypes.Text,
            };

            newServer.Channels = [channel];

            _context.Servers.Add(newServer);
            await _context.SaveChangesAsync();

            return CreatedAtAction("PostServer", new
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

        // Accept Server Invite
        // PATCH: api/servers/join/inviteCode
        [HttpPatch("invite/{inviteCode}/accept")]
        public async Task<IActionResult> AcceptServerInvite(string inviteCode)
        {
            var currentUser = await _context.Users.FindAsync(User.Identity!.Name);

            if (currentUser == null)
            {
                return Unauthorized();
            }

            // find server that invite code matches
            var serverFromDb = await _context.Servers.FirstOrDefaultAsync(x => x.InviteCode == inviteCode);

            if (serverFromDb == null)
            {
                return NotFound();
            }

            var member = await _context.Members.FirstOrDefaultAsync(x => x.UserId == currentUser.Id && x.ServerId == serverFromDb.Id);

            if (member != null)
            {
                return BadRequest();
            }

            // check if member is already in server
            if (serverFromDb.Members.Any(x => x.UserId == currentUser.Id))
            {
                return BadRequest();
            }

            Member newMember = new()
            {
                Id = Guid.NewGuid(),
                User = currentUser,
                UserId = currentUser.Id,
                MemberRole = Member.MemberRoles.Member,
                ServerId = serverFromDb.Id,
                Server = serverFromDb,

            };

            _context.Members.Add(newMember);

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

        // GET server from invite code
        [HttpGet("invite/{inviteCode}")]
        public async Task<IActionResult> GetServerFromInviteCode(string inviteCode)
        {
            var currentUser = await _context.Users.FindAsync(User.Identity!.Name);

            if (currentUser == null)
            {
                return Unauthorized();
            }

            var serverFromDb = await _context.Servers.FirstOrDefaultAsync(x => x.InviteCode == inviteCode);

            if (serverFromDb == null)
            {
                return NotFound();
            }
            // 
            // check if user is a member in the server
            var userIsMember = await _context.Members.AnyAsync(x => x.UserId == currentUser.Id && x.ServerId == serverFromDb.Id);

            // get member count
            var memberCount = await _context.Members.CountAsync(x => x.ServerId == serverFromDb.Id);

            return Ok(new { serverFromDb.Id, serverFromDb.Name, serverFromDb.ImageUrl, MemberCount = memberCount, UserIsMember = userIsMember });
        }

        private bool ServerExists(Guid id)
        {
            return _context.Servers.Any(e => e.Id == id);
        }
    }
}
