using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Esimerkki4.Kirjautuminen.Db;
using Esimerkki4.Kirjautuminen.Models;
using Microsoft.EntityFrameworkCore;

namespace Esimerkki4.Kirjautuminen.Stores {
    public class InvoiceStore
    {
        private readonly AppDbContext dbContext;

        public InvoiceStore(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task Update(Invoice invoice) {
            invoice.Modified = DateTime.UtcNow;
            var newRowIds = invoice.InvoiceRows.Select(t => t.Id).ToArray();

            // Update:n kutsuminen ei itseasiassa ole pakollista, sillä EF Core
            // osaa hallita olioiden muutoksia sisäisesti. Sisäinen hallinta voi
            // olla joskus varsin ongelmallista, sillä ei ole helposti
            // ennustettavissa mitä seuraava SaveChangesAsync() tekee.
            dbContext.Invoice.Update(invoice); 

            var removedRows = await dbContext
                .InvoiceRow
                .Where(t => 
                    t.InvoiceId == invoice.Id &&
                    !newRowIds.Contains(t.Id)
                )
                .ToListAsync();

            dbContext.RemoveRange(removedRows);
            
            // Tämä ajaa SQL kyselyt ja siihen asti kerätyt muutokset
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
                .Include(t => t.InvoiceRows)
                .Include(t => t.Client)
                .FirstOrDefaultAsync(t => t.Id == id && t.BusinessId == businessId);

            value.InvoiceRows = value.InvoiceRows.OrderBy(t => t.Sort).ToList();

            if (value == null) {
                throw new NotFoundException();
            }
            return value;
        }

        public async Task<ICollection<Invoice>> ListLatestByBusiness(int businessId) {
            return await dbContext.Invoice
                .Include(t => t.InvoiceRows) // Sisällytä kyselyyn laskurivien tiedot
                .Include(t => t.Client) // Sisällytä kyselyyn asiakkaantiedot
                .Where(t => t.BusinessId == businessId)
                .OrderBy(t => t.Created)
                .ToListAsync();
        }

        public async Task<bool> OwnedByBusiness(int businessId, int invoiceId) {
            return await dbContext.Invoice
                .AnyAsync(t => t.BusinessId == businessId && t.Id == invoiceId);
        }

        public async Task<bool> InvoiceRowsBelongToInvoice(int invoiceId, 
            IEnumerable<int> invoiceRowIds)
        {
            var existingInvoiceRowIds = await dbContext.InvoiceRow
                .Where(t => t.InvoiceId == invoiceId)
                .Select(t => t.Id)
                .ToListAsync();
            
            // Tarkista että lasku id:t kuuluvat olemassa olevien lasku id:n joukkoon
            return invoiceRowIds.All(t => existingInvoiceRowIds.Contains(t));
        }
    }
}