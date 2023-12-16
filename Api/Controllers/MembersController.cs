using Api.Data;
using Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers
{
    [Authorize]
    [Route("api/servers/")]
    [ApiController]
    public class MembersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public MembersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Members/5
        [HttpGet("{serverId}/members/{memberId}")]
        public async Task<ActionResult<Member>> GetMember(string memberId, string serverId)
        {

            var requestUserIsAdmin = await RequestUserIsAdmin(User.Identity!.Name!, memberId, serverId);

            if (!requestUserIsAdmin)
            {
                return Unauthorized();
            }

            var member = await _context.Members.FindAsync(Guid.Parse(memberId));

            if (member == null)
            {
                return NotFound();
            }

            return member;
        }

        // PUT: api/Members/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPatch("{serverId}/members/{memberId}")]
        public async Task<IActionResult> PutMember(string memberId, Member member, string serverId)
        {
            var requestUserIsAdmin = await RequestUserIsAdmin(User.Identity!.Name!, memberId, serverId);

            if (!requestUserIsAdmin)
            {
                return Unauthorized();
            }

            if (!MemberExists(memberId, serverId))
            {
                return NotFound();
            }

            var memberToUpdate = await _context.Members.FindAsync(Guid.Parse(memberId));
            if (memberToUpdate == null)
            {
                return NotFound();
            }

            if (member.MemberRole != memberToUpdate.MemberRole)
            {
                memberToUpdate.MemberRole = member.MemberRole;
            }

            _context.Entry(memberToUpdate).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Members/5
        [HttpDelete("{serverId}/members/{memberId}")]
        public async Task<IActionResult> DeleteMember(string memberId, string serverId)
        {
            var requestUserIsAdmin = await RequestUserIsAdmin(User.Identity!.Name!, memberId, serverId);

            if (!requestUserIsAdmin)
            {
                return Unauthorized();
            }

            var member = await _context.Members.FindAsync(Guid.Parse(memberId));
            if (member == null)
            {
                return NotFound();
            }

            _context.Members.Remove(member);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Members/5
        [HttpDelete("{serverId}/leave")]
        public async Task<IActionResult> RemoveSelf(string serverId)
        {
            var currentUser = await _context.Users.FindAsync(User.Identity!.Name!);

            if (currentUser == null)
            {
                return NotFound();
            }

            // check if member is in server
            var member = await _context.Members.FirstOrDefaultAsync(m => m.UserId == currentUser.Id && m.ServerId == Guid.Parse(serverId));

            if (member == null)
            {
                return NotFound();
            }

            _context.Members.Remove(member);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private async Task<bool> RequestUserIsAdmin(string requestUserId, string memberId, string serverId)
        {
            var user = await _context.Users.FindAsync(requestUserId);

            if (user == null)
            {
                return false;
            }

            // check if the user is trying to act on themselves
            if (user.Id == memberId)
            {
                return false;
            }

            // find the request users role in the server
            var member = await _context.Members.FirstOrDefaultAsync(m => m.UserId == requestUserId && m.ServerId == Guid.Parse(serverId));

            if (member == null)
            {
                return false;
            }

            return member.MemberRole == Member.MemberRoles.Admin;

        }

        private bool MemberExists(string memberId, string serverId)
        {
            // find if the member exists in the server
            return _context.Members.Any(e => e.Id == Guid.Parse(memberId) && e.ServerId == Guid.Parse(serverId));
        }
    }
}
