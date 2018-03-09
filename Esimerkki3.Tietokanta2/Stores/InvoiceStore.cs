using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Esimerkki3.Tietokanta2.Db;
using Esimerkki3.Tietokanta2.Models;
using Microsoft.EntityFrameworkCore;

namespace Esimerkki3.Tietokanta2.Stores {
    public class InvoiceStore
    {
        private readonly AppDbContext dbContext;

        public InvoiceStore(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task Update(Invoice invoice) {
            invoice.Modified = DateTime.UtcNow;
            dbContext.Invoice.Update(invoice);
            await dbContext.SaveChangesAsync();
        }

        public async Task<Invoice> Create(Invoice invoice) {
            invoice.Created = DateTime.UtcNow;
            invoice.Modified = DateTime.UtcNow;
            dbContext.Invoice.Add(invoice);
            await dbContext.SaveChangesAsync();
            return invoice;
        }

        public async Task Remove(Invoice invoice) {
            dbContext.Invoice.Remove(invoice);
            await dbContext.SaveChangesAsync();
        }

        public async Task<Invoice> GetByBusiness(int businessId, int id) {
            var value = await dbContext.Invoice
                .FirstOrDefaultAsync(t => t.Id == id && t.BusinessId == businessId);
            if (value == null) {
                throw new NotFoundException();
            }
            return value;
        }

        public async Task<ICollection<Invoice>> ListLatestByBusiness(int businessId) {
            return await dbContext.Invoice
                .Where(t => t.BusinessId == businessId)
                .Include(t => t.InvoiceRows) // Sisällytä kyselyyn laskurivien tiedot
                .Include(t => t.Client) // Sisällytä kyselyyn asiakkaantiedot
                .OrderBy(t => t.Created)
                .ToListAsync();
        }
    }
}