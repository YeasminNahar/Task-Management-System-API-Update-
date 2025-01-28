using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Task_Management_System_API.Attributes;
using Task_Management_System_API.Models;
using Task_Management_System_API.ViewModels;

namespace Task_Management_System_API.Controllers
{
    //[Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {

        // GET: api/Companies/5
        [HttpGet("api/GetUserDetail/{id}")]
        public async Task<ActionResult<UserDetail>> GetUserDetail(int id)
        {
            TaskDbContext _context = new TaskDbContext();
            var userDetail = await _context.UserDetails.FindAsync(id);

            if (userDetail == null)
            {
                return NotFound();
            }

            return userDetail;
        }

        // POST: api/Login
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
       
        [EnableCors("Policy1")]
        [HttpPost("api/login")]
       
        public async Task<IActionResult> PostUserDetail(LoginVm userDetail)
        {
            TaskDbContext _context = new TaskDbContext();
            var oUserDetail = await (from ud in _context.UserDetails.Where(x => x.UserName == userDetail.UserName && x.PassWord == userDetail.PassWord)
                                      join ur in _context.UserRoles on ud.RoleId equals ur.RoleId
                                      select new UserInfoViewModel
                                      {
                                          UserId=ud.UserId,
                                          UserName=ud.UserName,
                                          MemberId=ud.MemberId,
                                          RoleId=ur.RoleId,
                                          RoleName=ur.RoleName
                                      }).FirstOrDefaultAsync();


            if (oUserDetail == null)
            {
                return Ok("User Not Found");
            }
            else
            {
                if (oUserDetail != null)
                {
                    #region Token
                    var MyConfig = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
                    var WebClient = MyConfig.GetValue<string>("WebClient");
                    var TokenExpire = MyConfig.GetValue<int>("TokenExpire");
                    oUserDetail.Token = JsonWebToken.Encode(new UserPayload() { CreateDate = DateTime.Now, UserId = oUserDetail.UserId, TokenExpire = TokenExpire, UserName = oUserDetail.UserName }, WebClient, JwtHashAlgorithm.HS512);
                    #endregion
                }
                return Ok( oUserDetail);
            }
        }

        private bool UserDetailExists(int id)
        {
            TaskDbContext _context = new TaskDbContext();
            return _context.UserDetails.Any(e => e.UserId == id);
        }
    }

}
