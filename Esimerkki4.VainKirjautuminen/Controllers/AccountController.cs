using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Esimerkki4.Kirjautuminen.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Esimerkki4.Kirjautuminen.Controllers
{
    [Authorize] // <-- Huomaa tämä!
    [Route("[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> userManager;

        public AccountController(UserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager;
        }

        [HttpGet("[action]")]
        public object Claims() {
            // Näyttää kirjautuneen käyttäjän vaateet, tämä demonstroi sitä mitä
            // vaateet tarkoittavat. User on ClaimsPrincipal tyyppinen olio, ei
            // siis ApplicationUser.
            if (User == null) {
                return null;
            }

            return User.Claims.Select(t => new { t.Type, t.Value }).ToList();
        }

        public class LoggedInDto {
            public int Id { get; set; } = 0;
            public string Email { get; set; } = "";
        }

        [HttpGet("[action]")]
        public async Task<LoggedInDto> LoggedIn() {
            // Näyttää kirjautuneen käyttäjän tiedot

            var loggedInUser = await userManager.GetUserAsync(User);
            if (loggedInUser == null) {
                return null;
            }

            return new LoggedInDto() {
                Id = loggedInUser.Id,
                Email = loggedInUser.Email
            };
        }
    }
}
