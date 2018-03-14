using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Esimerkki4.Kirjautuminen.Db;
using Esimerkki4.Kirjautuminen.Models;
using Microsoft.EntityFrameworkCore;

namespace Esimerkki4.Kirjautuminen.Stores {
    public class EmailStore
    {
        private readonly AppDbContext dbContext;

        public EmailStore(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<Email> Create(Email email) {
            email.Created = DateTime.UtcNow;
            dbContext.Email.Add(email);
            await dbContext.SaveChangesAsync();
            return email;
        }
    }
}