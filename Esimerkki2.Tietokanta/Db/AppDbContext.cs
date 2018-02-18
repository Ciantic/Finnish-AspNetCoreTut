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
        
        // Taulujen nimiksi tulee seuraavien propertyjen nimet, esim "Business" tai "Client"
        public DbSet<Business> Business { get; set; }
        public DbSet<Client> Client { get; set; }
        public DbSet<Invoice> Invoice { get; set; }
        public DbSet<InvoiceRow> InvoiceRow { get; set; }
        public DbSet<Email> Email { get; set; }

        protected override void OnModelCreating(ModelBuilder builder) {
            // Täällä voi antaa eri asetuksia tauluille
            // Esim. seuraava antaisi yhdistetyn avaimen
            // builder.Entity<Business>().HasAlternateKey(t => new { t.Id, t.OwnerApplicationUserId })
            
            // Tai seuraavalla voisi antaa kummallisen nimen taululle
            // builder.Entity<Business>().ToTable("OUTOLINTU");
        }
    }
}