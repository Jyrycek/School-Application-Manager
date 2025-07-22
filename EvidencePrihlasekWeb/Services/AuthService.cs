using EvidencePrihlasekWeb.Models;
using Database;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;

namespace EvidencePrihlasekWeb.Services
{
    public class AuthService
    {
        public async Task RegisterUser(Users user)
        {
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);

            await DatabaseManager.AddNewUser(user);
        }

        public async Task LoginUser(Users user, HttpContext httpContext)
        {
            List<Claim> claims = new List<Claim>
            {
            new Claim(ClaimTypes.Name, user.Username)
            };

            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties();

            await httpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity),
            authProperties);
        }
    }
}
