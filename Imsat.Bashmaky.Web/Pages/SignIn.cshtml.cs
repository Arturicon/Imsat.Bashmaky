using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using System;
using Imsat.Bashmaky.Web.Db;
using Imsat.Bashmaky.Web.WebApi.Dto;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;

namespace Imsat.Bashmaky.Web.Pages
{
    public class SignInModel : PageModel
    {        
        public async Task<IActionResult> OnPost(string? returnUrl, [FromServices] UserDbContext userDbContext) 
        {
            var form = Request.Form;

            if (!form.ContainsKey("login") || !form.ContainsKey("password"))
                return BadRequest("Email и/или пароль не установлены");

            string login = form["login"];
            string password = form["password"];

            var user = await userDbContext.Users.FirstOrDefaultAsync(x => x.Login == login);
            if (user == null)
                return BadRequest("User with such login wasn't found");
            if (user.Password != password)
                return BadRequest("Password incorrect");

            var claims = new List<Claim> { new Claim(ClaimTypes.Name, login) };

            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "Cookies");
            // установка аутентификационных куки
            await Request.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
            return Redirect(returnUrl ?? "/");
        }

    }
}
