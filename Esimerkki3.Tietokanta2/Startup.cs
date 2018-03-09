using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Esimerkki3.Tietokanta2.Db;
using Esimerkki3.Tietokanta2.Models;
using Esimerkki3.Tietokanta2.Services;
using Esimerkki3.Tietokanta2.Stores;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Swagger;

namespace Esimerkki3.Tietokanta2
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

            services.AddMvc();

            services.AddDbContext<AppDbContext>(o => {
                o.UseSqlite(Configuration.GetConnectionString("Database"));
            });

            // Services
            services.AddTransient<AccountService, AccountService>();
            services.AddTransient<BusinessService, BusinessService>();
            services.AddTransient<ClientService, ClientService>();
            services.AddTransient<IEmailSender, EmailSender>();
            services.AddTransient<InvoiceService, InvoiceService>();
            services.AddTransient<NotificationSender, NotificationSender>();

            // Stores
            services.AddTransient<BusinessStore, BusinessStore>();
            services.AddTransient<ClientStore, ClientStore>();
            services.AddTransient<EmailStore, EmailStore>();
            services.AddTransient<InvoiceStore, InvoiceStore>();

            if (Environment.IsProduction()) {
                services.AddTransient<IInitDb, InitDbProduction>();
            } else if (Environment.IsDevelopment()) {
                services.AddTransient<IInitDb, InitDbDevelopment>();
            }

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

            using (var scoped = app.ApplicationServices.CreateScope()) {
                scoped.ServiceProvider.GetRequiredService<IInitDb>().Init().GetAwaiter().GetResult();
            }
        }
    }
}
