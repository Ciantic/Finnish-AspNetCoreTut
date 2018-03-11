using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Esimerkki2.Tietokanta.Db;
using Esimerkki2.Tietokanta.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Esimerkki2.Tietokanta.Controllers
{
    [Route("api/[controller]")]
    public class ClientsController : Controller
    {
        private readonly AppDbContext dbContext;

        public ClientsController(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public class CreateClientDto {
            [MinLength(3)]            
            public string Name { get; set; } = "";
            public string Address { get; set; } = "";
            public string City { get; set; } = "";

            [RegularExpression("(\\d{5})?")]
            public string PostCode { get; set; } = "";

            [EmailAddress]
            public string Email { get; set; } = "";

            public string PhoneNumber { get; set; } = "";
        }

        // Huomaa että palautusmuoto on object, joka on huono, mutta tämä on
        // yksinkertaisin esimerkki
        [HttpPost] 
        public async Task<object> Create([FromBody] CreateClientDto createClientDto)
        {   
            if (!ModelState.IsValid) {
                // Tälle tulee parempi vaihtoehto seuraavassa esimerkissä
                return ModelState;
            }
            // Business ID on vakiona 1 tässä esimerkissä
            var businessId = 1;

            var newClient = new Client() {
                City = createClientDto.City,
                Address = createClientDto.Address,
                Name = createClientDto.Name,
                PostCode = createClientDto.PostCode,
                Email = createClientDto.Email,
                BusinessId = businessId,
            };
            dbContext.Client.Add(newClient);
            await dbContext.SaveChangesAsync();
            return newClient;
        }
    }
}