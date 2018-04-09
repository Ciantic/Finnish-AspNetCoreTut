using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Esimerkki4.Kirjautuminen.Db;
using Esimerkki4.Kirjautuminen.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Swagger;
using IdentityServer4.Models;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authentication;
using System.Text.Encodings.Web;

namespace Esimerkki4.Kirjautuminen
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
            services.AddIdentity<ApplicationUser, ApplicationRole>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

            // Autentikaation asetuksia
            services
                .AddAuthentication(o =>
                {
                    // Nämä vaaditaan koska .NET Core 2 ohjaa oletuksena
                    // kirjautumisdialogiin, jota REST rajapinnoissa ei ole
                    o.DefaultScheme =
                        o.DefaultAuthenticateScheme =
                        o.DefaultForbidScheme =
                        o.DefaultChallengeScheme =
                        o.DefaultSignInScheme =
                        o.DefaultSignOutScheme =
                        IdentityServerAuthenticationDefaults.AuthenticationScheme;
                })
                .AddIdentityServerAuthentication(options => // IdentityServer4
                {
                    options.Authority = "http://localhost:5000";
                    options.RequireHttpsMetadata = false;
                    options.ApiName = "minunapi";
                });

            // Nämä ovat Identity serverin (eli authorisaation) konfiguraatiota
            services.AddIdentityServer()
                .AddDeveloperSigningCredential()
                .AddInMemoryPersistedGrants()
                .AddInMemoryApiResources(new List<IdentityServer4.Models.ApiResource>() {
                    new IdentityServer4.Models.ApiResource("minunapi", "Tämä ohjelma")
                })
                .AddInMemoryClients(new List<IdentityServer4.Models.Client>() {
                    // Open ID Perustuu asiakasohjelmiin, eli jokainen
                    // käyttöliittymä rekisteröidään API:lle. Esimerkiksi
                    // mobiilisovelluksella olisi oma client ja websovelluksella
                    // omansa.
                    new IdentityServer4.Models.Client() {
                        ClientId = "esimerkkiohjelma",
                        AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                        AllowOfflineAccess = true,
                        ClientSecrets = {
                            // JavaScript sovelluksen client secret ei ole kovin
                            // tärkeä sillä se näkyy JavaScript ohjelman
                            // lähdekoodeissa, se vuotaa joka tapauksessa.
                            new Secret("secret".Sha256())
                        },
                        AllowedScopes = { "minunapi" }
                    }
                }).AddAspNetIdentity<ApplicationUser>();

            services.AddMvc();

            services.AddDbContext<AppDbContext>(o =>
            {
                o.UseSqlite(Configuration.GetConnectionString("Database"));
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "My API", Version = "v1" });
                c.AddSecurityDefinition("oauth2", new OAuth2Scheme()
                {
                    Flow = "password",
                    Scopes = new Dictionary<string, string> {
                        { "minunapi", "Minun API oikeudet" }
                    },
                    TokenUrl = "http://localhost:5000/connect/token",
                    AuthorizationUrl = "http://localhost:5000/connect/authorize"
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseIdentityServer();
            app.UseAuthentication();
            app.UseMvc();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "API");
            });

            using (var scoped = app.ApplicationServices.CreateScope())
            {
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

            // Luo testidata development tilalle ...

            var acmeUser =  new ApplicationUser() {
                Email = "business@example.com",
                UserName = "business@example.com",
                PasswordHash = passwordHasher.HashPassword(null, "!Pass1"), // Testi käyttäjän salasana
                NormalizedEmail = "business@example.com".ToUpper(),
                NormalizedUserName = "business@example.com".ToUpper(),
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                SecurityStamp = Guid.NewGuid().ToString(),
                EmailConfirmed = true,
            };

            appDbContext.Users.Add(acmeUser);

            await appDbContext.SaveChangesAsync();
        }
    }
}
