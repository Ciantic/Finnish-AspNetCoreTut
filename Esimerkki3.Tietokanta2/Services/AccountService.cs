using System.Linq;
using System.Threading.Tasks;
using Esimerkki3.Tietokanta2.Db;
using Esimerkki3.Tietokanta2.Models;
using Esimerkki3.Tietokanta2.Stores;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Esimerkki3.Tietokanta2.Services {
    public class AccountService
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly NotificationSender notificationSender;

        public AccountService(UserManager<ApplicationUser> userManager, NotificationSender notificationSender)
        {
            this.userManager = userManager;
            this.notificationSender = notificationSender;
        }

        // Huomaa rekisteröintiä ei ole täällä, koska se on yrityskohtainen asia

        public async Task<bool> ForgotPassword(ApplicationUser user) {
            var resetToken = await userManager.GeneratePasswordResetTokenAsync(user);
            await notificationSender.SendForgotPassword(user, resetToken);
            return true;
        }

        public async Task<bool> ResetPassword(ApplicationUser user, string resetToken, string newPassword) {
            await userManager.ResetPasswordAsync(user, resetToken, newPassword);
            // Tässä voisi lähettää notifikaation että salasana on vaihdettu
            return true;
        }

        public async Task<bool> ChangePassword(ApplicationUser user, string currentPassword, string newPassword) {
            await userManager.ChangePasswordAsync(user, currentPassword, newPassword);
            // Tässä voisi lähettää notifikaation että salasana on vaihdettu
            return true;
        }
    }
}