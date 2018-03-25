using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Esimerkki4.Kirjautuminen.Controllers.Dtos;
using Esimerkki4.Kirjautuminen.Models;
using Esimerkki4.Kirjautuminen.Mvc;
using Esimerkki4.Kirjautuminen.Services;
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
        private readonly AccountService accountService;

        public AccountController(UserManager<ApplicationUser> userManager, AccountService accountService)
        {
            this.userManager = userManager;
            this.accountService = accountService;
        }

        public class LoggedInDto {
            public int Id { get; set; } = 0;
            public string Email { get; set; } = "";
        }

        [HttpGet("[action]")]
        public object Claims() {
            // Näyttää kirjautuneen käyttäjän vaateet, tämä demonstroi sitä mitä
            // vaateet tarkoittavat. User on ClaimsPrincipal tyyppinen olio, ei
            // siis ApplicationUser.
            if (User == null) {
                throw new Forbidden();
            }

            return User.Claims.Select(t => new { t.Type, t.Value }).ToList();
        }

        [HttpGet("[action]")]
        public async Task<LoggedInDto> LoggedIn() {
            // Näyttää kirjautuneen käyttäjän tiedot

            var loggedInUser = await userManager.GetUserAsync(User);
            if (loggedInUser == null) {
                throw new Forbidden();
            }

            return new LoggedInDto() {
                Id = loggedInUser.Id,
                Email = loggedInUser.Email
            };
        }

        public class ChangePasswordDto {
            public string CurrentPassword { get; set; } = "";
            public string NewPassword { get; set; } = "";
        }

        public class ChangePasswordError : ApiError<List<string>> {
            public ChangePasswordError (IEnumerable<IdentityError> errors)
            {
                JsonData = errors.Select(t => t.Description).ToList();
            }
        }

        [HttpPost("[action]")]
        public async Task<bool> ChangePassword([FromBody] ChangePasswordDto changePasswordDto) {
            var loggedInUser = await userManager.GetUserAsync(User);
            if (loggedInUser == null) {
                throw new Forbidden();
            }

            var result = await accountService.ChangePassword(loggedInUser, 
                changePasswordDto.CurrentPassword, 
                changePasswordDto.NewPassword);

            if (!result.Succeeded) {
                throw new ChangePasswordError(result.Errors);    
            }

            return true;
        }

        public class ForgotPasswordDto {

            [EmailAddress]
            public string Email { get; set; } = "";
        }

        // Huomaa tämä! Tämä sallii kaikki käyttäjät Authorize attribuutista huolimatta
        [AllowAnonymous]
        [HttpPost("[action]")]
        public async Task<bool> ForgotPassword([FromBody] ForgotPasswordDto forgotPasswordDto) {
            var foundUser = await userManager.FindByEmailAsync(forgotPasswordDto.Email);
            if (foundUser == null) {
                // Tässä voi myös palauttaa true jos haluaa että tällä ei voi
                // pollata käyttäjiä
                return false;
            }

            await accountService.ForgotPassword(foundUser);
            return true;
        }

        public class ResetPasswordDto {
            public string ResetPasswordToken { get; set; } = "";
            public string NewPassword { get; set; } = "";
        }

        public class ResetPasswordError : ApiError<List<string>> {
            public ResetPasswordError (IEnumerable<IdentityError> errors)
            {
                JsonData = errors.Select(t => t.Description).ToList();
            }
        }

        // Huomaa tämä! Tämä sallii kaikki käyttäjät Authorize attribuutista huolimatta
        [AllowAnonymous]
        [HttpPost("[action]")]
        public async Task<bool> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto) {
            var loggedInUser = await userManager.GetUserAsync(User);
            if (loggedInUser == null) {
                throw new Forbidden();
            }

            var result = await accountService.ResetPassword(loggedInUser, 
                resetPasswordDto.ResetPasswordToken, 
                resetPasswordDto.NewPassword);
            
            if (!result.Succeeded) {
                throw new ResetPasswordError(result.Errors);
            }

            return true;
        }
    }
}
