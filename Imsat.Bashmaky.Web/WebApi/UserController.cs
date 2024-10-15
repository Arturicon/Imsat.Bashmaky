using Imsat.Bashmaky.Web.Db;
using Imsat.Bashmaky.Web.WebApi.Dto;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;

namespace Imsat.Bashmaky.Web.WebApi
{
    [ApiController()]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly UserDbContext _userDbContext;

        public UserController([FromServices] UserDbContext userDbContext)
        {
            _userDbContext = userDbContext;

        }

        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
        [HttpPost("[action]")]
        public async Task<ActionResult> AddUser([FromBody] AddUserRequest addRequest)
        {
            if (await _userDbContext.Users.AnyAsync(x => x.Login == addRequest.Login))
            {
                return BadRequest("User with such login already exist!");
            }
            var newUser = new User { Login = addRequest.Login, Password = addRequest.Password, Name = addRequest.Name };
            await _userDbContext.AddAsync(newUser);
            await _userDbContext.SaveChangesAsync();
            return Ok(newUser);
        }


        [HttpPost("[action]")]
        public async Task<ActionResult> DeleteUser([FromQuery] int userId)
        {
            var user = await _userDbContext.FindAsync<User>(userId);
            if (user == null)
                return NotFound("User with such id not found");
            _userDbContext.Remove(user);
            await _userDbContext.SaveChangesAsync();
            return Ok();
        }

        //[HttpGet("[action]")]
        //public async Task<ActionResult> CheckAuthentication()
        //{
        //    var user = HttpContext.User.Identity;
        //    if (user is not null && user.IsAuthenticated)
        //    {
        //        return Ok();
        //    }
        //    return NotFound("User isn't authenticated");
        //}

        [HttpPost("[action]")]
        public async Task Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }

    }
}
