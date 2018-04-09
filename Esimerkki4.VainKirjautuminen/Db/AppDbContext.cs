using Esimerkki4.Kirjautuminen.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Esimerkki4.Kirjautuminen.Db
{
    public class AppDbContext : IdentityDbContext<
        ApplicationUser, ApplicationRole, int
    >
    {

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            
        }
    }
}