using System.Linq;
using System.Threading.Tasks;
using Esimerkki3.Tietokanta2.Db;
using Esimerkki3.Tietokanta2.Models;
using Esimerkki3.Tietokanta2.Stores;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Esimerkki3.Tietokanta2.Services {
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