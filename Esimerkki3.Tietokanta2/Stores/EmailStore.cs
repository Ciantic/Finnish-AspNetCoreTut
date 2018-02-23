using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Esimerkki3.Tietokanta2.Db;
using Esimerkki3.Tietokanta2.Models;
using Microsoft.EntityFrameworkCore;

namespace Esimerkki3.Tietokanta2.Stores {
    public class EmailStore
    {
        private readonly AppDbContext dbContext;

        public EmailStore(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<Email> Create(Email email) {
            dbContext.Email.Add(email);
            await dbContext.SaveChangesAsync();
            return email;
        }
    }
}