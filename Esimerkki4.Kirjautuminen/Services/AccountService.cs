using System.Linq;
using System.Threading.Tasks;
using Esimerkki4.Kirjautuminen.Db;
using Esimerkki4.Kirjautuminen.Models;
using Esimerkki4.Kirjautuminen.Stores;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Esimerkki4.Kirjautuminen.Services {
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

        public async Task<IdentityResult> ResetPassword(ApplicationUser user, string resetToken, string newPassword) {
            var result = await userManager.ResetPasswordAsync(user, resetToken, newPassword);
            
            if (result.Succeeded) {
                // Ilmoita käyttäjälle sähköpostitse että salasana on vaihdettu
                await notificationSender.SendPasswordChanged(user);
            }
            return result;
        }

        public async Task<IdentityResult> ChangePassword(ApplicationUser user, string currentPassword, string newPassword) {
            var result = await userManager.ChangePasswordAsync(user, currentPassword, newPassword);

            if (result.Succeeded) {
                // Ilmoita käyttäjälle sähköpostitse että salasana on vaihdettu
                await notificationSender.SendPasswordChanged(user);
            }

            return result;
        }
    }
}