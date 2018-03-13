using Esimerkki3.Tietokanta2.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Esimerkki3.Tietokanta2.Db
{
    public class AppDbContext : DbContext
    {
        private readonly ILoggerFactory loggerFactory;

        public AppDbContext(DbContextOptions<AppDbContext> options, ILoggerFactory loggerFactory) : base(options)
        {
            this.loggerFactory = loggerFactory;
        }
        
        public DbSet<Business> Business { get; set; }
        public DbSet<Client> Client { get; set; }
        public DbSet<Invoice> Invoice { get; set; }

        public DbSet<InvoiceRow> InvoiceRow { get; set; }

        public DbSet<Email> Email { get; set; }

        override protected void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.EnableSensitiveDataLogging();
            optionsBuilder.UseLoggerFactory(loggerFactory);
        }
    }
}