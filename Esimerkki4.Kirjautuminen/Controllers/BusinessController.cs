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
    [Route("[controller]")]
    public class BusinessController : ControllerBase
    {
        private readonly BusinessService businessService;

        public BusinessController(BusinessService businessService)
        {
            this.businessService = businessService;
        }

        public class BusinessRegisterDto {

            [EmailAddress]
            public string Email { get; set; } = "";

            [MinLength(3)]
            public string Title { get; set; } = "";

            [MinLength(2)]
            public string FirstName { get; set; } = "";

            [MinLength(2)]
            public string LastName { get; set; } = "";

            [MinLength(2)]
            public string Password { get; set; } = "";
        }

        public class RegisterError : ApiError<List<string>> {
            public RegisterError (IEnumerable<IdentityError> errors)
            {
                JsonData = errors.Select(t => t.Description).ToList();
            }
        }

        [HttpPost("[action]")]
        public async Task<bool> Register([FromBody] BusinessRegisterDto businessRegisterDto) {
            var result = await businessService.Register(
                email: businessRegisterDto.Email, 
                password: businessRegisterDto.Password,
                title: businessRegisterDto.Title,
                firstName: businessRegisterDto.FirstName,
                lastName: businessRegisterDto.LastName);
                
            if (!result.Succeeded) {
                throw new RegisterError(result.Errors);
            }

            return true;
        }
    }
}
