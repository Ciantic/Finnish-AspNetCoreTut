using System;
using System.Linq;
using System.Threading.Tasks;
using Esimerkki4.Kirjautuminen.Db;
using Esimerkki4.Kirjautuminen.Models;
using Esimerkki4.Kirjautuminen.Stores;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Esimerkki4.Kirjautuminen.Services {
    public class NotificationSender
    {
        private readonly IEmailSender emailSender;

        public NotificationSender(IEmailSender emailSender)
        {
            this.emailSender = emailSender;
        }

        public async Task SendBusinessRegistered(Business business) {
            // Notifikaatioillekkin voi tehdä oman taulun tarpeen vaatiessa,
            // linkittää sähköposti ja yritys sitä kautta keskenään. Tässä
            // esimerkissä notifikaatioilla ei ole omaa taulua.
            
            await emailSender.SendEmailAsync(new Email() {
                From = "palvelu@example.com",
                Subject = "Rekisteröidyit palveluun",
                Body = "Rekisteröidyit palveluun",
                To = business.OwnerApplicationUser.Email
            });
        }

        public async Task SendForgotPassword(ApplicationUser user, string resetUrl) {
            await emailSender.SendEmailAsync(new Email() {
                From = "palvelu@example.com",
                Subject = "Unohditko salasanasi?",
                Body = $"Unohditko salasanasi? Resetoi se tästä: {resetUrl}",
                To = user.Email
            });
        }

        public async Task SendInvoice(Invoice invoice) {
            if (invoice.Client == null)
            {
                // Tämä on API:n käyttövirhe
                throw new ArgumentException("Invoice Client must be set for sending", nameof(invoice));
            }

            await emailSender.SendEmailAsync(new Email() {
                From = "palvelu@example.com",
                Subject = "Laskusi!",
                Body = $"Laskusi, ole hyvä...",
                To = invoice.Client.Email
            });
        }
    }
}