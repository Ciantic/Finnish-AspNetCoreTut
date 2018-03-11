using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Esimerkki2.Tietokanta.Db;
using Esimerkki2.Tietokanta.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Esimerkki2.Tietokanta.Controllers
{
    [Route("api/[controller]")]
    public class InvoicesController
    {
        private readonly AppDbContext dbContext;

        public InvoicesController(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpGet("{id}")] 
        public async Task<Invoice> GetById(int id)
        {   
            // Tässä esimerkissä palautetaan modeli, normaalisti sitä ei
            // tehtäisi vaan jokaiselle end pointille määritellään oma
            // palautusluokka, modelit eivät saa vuotaa rajapinnalle
            return await dbContext.Invoice.Where(t => t.Id == id)
                .Include(t => t.InvoiceRows)
                .FirstOrDefaultAsync();
        }
    }
}