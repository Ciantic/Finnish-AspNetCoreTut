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

        public async Task Register(ApplicationUser user, Business business)
        {
            // Seuraavat kaksi voisi tehdä tietokanta transactionissa
            await userManager.CreateAsync(user);
            await businessStore.Create(business);
            await notificationSender.SendBusinessRegistered(business);
        }
    }
}