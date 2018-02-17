using Esimerkki2.Tietokanta.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Esimerkki2.Tietokanta.Db
{
    public class AppDbContext : DbContext
    { 
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        
        public DbSet<Business> Business { get; set; }
        public DbSet<Client> Client { get; set; }
        public DbSet<Invoice> Invoice { get; set; }

        public DbSet<InvoiceRow> InvoiceRow { get; set; }

        public DbSet<Email> Email { get; set; }
    }
}