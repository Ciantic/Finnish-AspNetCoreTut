using System;
using System.Linq;
using System.Threading.Tasks;
using Esimerkki4.Kirjautuminen.Db;
using Esimerkki4.Kirjautuminen.Models;
using Esimerkki4.Kirjautuminen.Stores;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Esimerkki4.Kirjautuminen.Services {
    public class BusinessService
    {
        private readonly BusinessStore businessStore;
        private readonly NotificationSender notificationSender;
        private readonly UserManager<ApplicationUser> userManager;

        public BusinessService(BusinessStore businessStore, NotificationSender notificationService, UserManager<ApplicationUser> userManager)
        {
            this.businessStore = businessStore;
            this.notificationSender = notificationService;
            this.userManager = userManager;
        }

        public async Task<IdentityResult> Register(string email, string password, string title, string firstName, string lastName)
        {
            // Luo käyttäjä
            var user = new ApplicationUser { 
                UserName = email, 
                Email = email,
                FirstName = firstName,
                LastName = lastName
            };
            var result = await userManager.CreateAsync(user, password);
            if (!result.Succeeded) {
                return result;
            }

            // Luo yritys
            var business = new Business() {
                OwnerApplicationUser = user,
                Title = title,
            };
            await businessStore.Create(business);

            // Lähetä sähköposti, jossa pyydetään mm varmistamaan sähköpostiosoite
            var emailConfirmToken = await userManager.GenerateEmailConfirmationTokenAsync(user);                
            await notificationSender.SendBusinessRegistered(business, emailConfirmToken);            
            return result;
        }
    }
}