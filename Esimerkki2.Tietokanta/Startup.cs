using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Esimerkki2.Tietokanta.Db;
using Esimerkki2.Tietokanta.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Swagger;

namespace Esimerkki2.Tietokanta
{
    public class Startup
    {

        public IConfiguration Configuration { get; }

        public IHostingEnvironment Environment { get; }
        
        public Startup(IHostingEnvironment env, IConfiguration configuration)
        {
            Configuration = configuration;
            Environment = env;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<AppDbContext>(o => {
                // SQLite tietokannan nimi
                o.UseSqlite("Data Source=esimerkki.development.db;");
            });

            services.AddIdentity<ApplicationUser, ApplicationRole>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

            services.AddMvc();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "My API", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseMvc();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "API");
            });

            // AppDbContextin voi luoda vain scopen sisällä, joten ensin luodaan scope
            using (var scoped = app.ApplicationServices.CreateScope()) {
                var dbContext = scoped.ServiceProvider.GetRequiredService<AppDbContext>();
                var passHasher = scoped.ServiceProvider.GetRequiredService<IPasswordHasher<ApplicationUser>>();

                // Configure on synkroninen, joten tässä pitää odotella
                CreateTestData(dbContext, passHasher).GetAwaiter().GetResult();
            }
        }

        private async Task CreateTestData(AppDbContext appDbContext, 
            IPasswordHasher<ApplicationUser> passwordHasher)
        {
            // Tuhoaa ja poistaa tietokannan joka kerta
            await appDbContext.Database.EnsureDeletedAsync();
            await appDbContext.Database.EnsureCreatedAsync();

            // Luo testidata development tilalle

            var acmeUser =  new ApplicationUser() {
                Email = "business@example.com",
                UserName = "business@example.com",
                PasswordHash = passwordHasher.HashPassword(null, "!Pass1"),
                NormalizedEmail = "business@example.com".ToUpper(),
                NormalizedUserName = "business@example.com".ToUpper(),
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                SecurityStamp = Guid.NewGuid().ToString(),
                EmailConfirmed = true,
            };

            var acmeBusiness = new Business() {
                Title = "Acme Inc",
                OwnerApplicationUser = acmeUser,
            };

            var clients = new List<Client>() {
                new Client() {
                    Business = acmeBusiness,
                    Name = "Kukkaismyynti Oy",
                    Address = "Kukkaiskuja 3",
                    City = "Jyväskylä",
                    PostCode = "40100",
                    Email = "kukkaismyynti@example.com",
                    PhoneNumber = "+3585012341234"
                },
                new Client() {
                    Business = acmeBusiness,
                    Name = "Kynäkauppiaat Ry",
                    Address = "Kynäkatu 123",
                    City = "Helsinki",
                    PostCode = "00100",
                    Email = "kynakauppias@example.com",
                    PhoneNumber = "+3585043214321"
                }
            };

            var invoices = new List<Invoice>() {
                new Invoice() {
                    Business = acmeBusiness, 
                    Client = clients[0],
                    Title = "Lasku ruusupuskista",
                    InvoiceRows = new List<InvoiceRow>() {
                        new InvoiceRow() {
                            Amount = 15.0M,
                            Created = DateTime.Now,
                            Modified = DateTime.Now,
                            Name = "Ruuspuskan siemenet",
                            Quantity = 123
                        }
                    },
                    Modified = DateTime.Now,
                    Created = DateTime.Now,
                    Sent = null,
                }
            };

            appDbContext.Business.Add(acmeBusiness);
            appDbContext.Client.AddRange(clients);
            appDbContext.Invoice.AddRange(invoices);
            
            await appDbContext.SaveChangesAsync();
        }
    }
}
