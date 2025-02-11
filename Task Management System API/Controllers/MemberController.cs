using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;
using Task_Management_System_API.Models;
using Task_Management_System_API.Services;
using Task_Management_System_API.ViewModels;

namespace Task_Management_System_API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class MemberController : ControllerBase
    {
        private readonly TaskDbContext db;
        private readonly IEmailService _emailService;

        public MemberController(TaskDbContext context,IEmailService emailService)
        {
            db = context;
            _emailService = emailService;
        }

        // GET: api/Member
        [HttpGet]
        [ProducesResponseType(typeof(Member[]), 200)]
        public async Task<ActionResult<Member[]>> GetMember()
        {
            var data = await db.Members.ToListAsync();
            return Ok(data);
        }

        // GET: api/Member/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetMember(int id)
        {
            var member = await db.Members.FindAsync(id);
            if (member == null) return NotFound();
            return Ok(member);
        }
        [HttpPost]
        public async Task<IActionResult> CreateMember(MemberViewModel yeasmin)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    // Log validation errors
                    var errors = ModelState.SelectMany(ms => ms.Value.Errors)
                                           .Select(e => e.ErrorMessage)
                                           .ToList();

                    return BadRequest(new { Errors = errors });
                }
                var member = new Member
                {
                    MemberName = yeasmin.MemberName,
                    Email = yeasmin.Email,
                    Password = yeasmin.Password,
                    CreateBy = yeasmin.CreateBy, // Handle logged-in user
                    UpdateBy = yeasmin.UpdateBy, // Handle logged-in user
                    CreateDate = DateTime.Now, // Set the creation date
                    UpdateDate = DateTime.Now, // Set the creation date
                    IsActive = true,
                };

                // Default value

                db.Members.Add(member);
                await db.SaveChangesAsync();
                var mId = member.MemberId;

                if (mId > 0)
                {
                    var userer = new UserDetail
                    {
                        MemberId = mId,
                        UserName = member.MemberName,
                        PassWord = yeasmin.Password,
                        RoleId = db.UserRoles.Where(x => x.RoleName == "Member").FirstOrDefault().RoleId
                    };

                    db.UserDetails.Add(userer);
                    await db.SaveChangesAsync();
                    string emailBody = $@"
             <p>Welcome</p>
                <p>Your account has been successfully created. Below are your login credentials:</p>
             <p>This is your Login Username: <strong>{userer.UserName}</strong></p>
             
              <p>Password:<strong>{userer.PassWord}</strong></p>  
             <p>Best regards,<br/>Task Management System</p>";

                    // Verify the email
                    if (!IsValidEmail(yeasmin.Email))
                    {
                        Console.WriteLine($"Invalid email address: {yeasmin.Email}");
                        return BadRequest("The email address is invalid.");
                    }

                    // Check if the email is not empty and is a valid address
                    if (string.IsNullOrEmpty(yeasmin.Email) || !member.Email.Contains("@"))
                    {
                        return BadRequest("Invalid email address.");
                    }

                    await _emailService.SendEmailAsync(yeasmin.Email, "New Member created ", emailBody);
                }
            

                return Ok(new {MemberName=member.MemberName,Email=member.Email });
            }
            catch (Exception ex)
            {

                return Ok(ex.Message.ToString());
            }
            
        }
        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMember(int id, [FromBody] Member updatedMember)
        {
            if (id != updatedMember.MemberId)
                return BadRequest("Member ID mismatch.");

            var existingMember = await db.Members.FindAsync(id);
            if (existingMember == null)
                return NotFound("MemberId not found.");

            // Update the Category properties
            existingMember.MemberName = existingMember.MemberName;
            existingMember.Email=existingMember.Email;
            existingMember.Password=existingMember.Password;
            existingMember.UpdateBy = existingMember.UpdateBy;
            existingMember.UpdateDate = DateTime.Now; // Set the current date for update
            existingMember.IsActive = existingMember.IsActive;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await MemberExists(id))
                    return NotFound("Member not found during update.");
                throw;
            }
            catch (Exception ex)
            {
                // Log the exception (could use a logger here, but for simplicity returning it)
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while updating the task category.", details = ex.Message });
            }

            return NoContent();
        }


        private async Task<bool> MemberExists(int id)
        {
            return await db.Members.AnyAsync(m => m.MemberId == id);
        }

        // DELETE: api/Member/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMember(int id)
        {
            var member = await db.Members.FindAsync(id);
            if (member == null) return NotFound();
            db.Members.Remove(member);
            await db.SaveChangesAsync();
            return NoContent();
        }
    }
}