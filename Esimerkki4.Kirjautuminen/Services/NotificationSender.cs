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

        public async Task SendBusinessRegistered(Business business, string emailConfirmToken) {
            // Notifikaatioillekkin voi tehdä oman taulun tarpeen vaatiessa,
            // linkittää sähköposti ja yritys sitä kautta keskenään. Tässä
            // esimerkissä notifikaatioilla ei ole omaa taulua.
            
            await emailSender.SendEmailAsync(new Email() {
                From = "palvelu@example.com",
                Subject = "Rekisteröidyit palveluun",
                Body = $@"Rekisteröidyit palveluun token, 
                    (oikeassa ohjelmassa tämä olisi URL jossa token on parametrina): 
                    {emailConfirmToken}",
                To = business.OwnerApplicationUser.Email
            });
        }

        public async Task SendForgotPassword(ApplicationUser user, string resetToken) {
            await emailSender.SendEmailAsync(new Email() {
                From = "palvelu@example.com",
                Subject = "Unohditko salasanasi?",
                Body = $@"Unohditko salasanasi? 
                    (oikeassa ohjelmassa tämä olisi URL jossa token on parametrina): 
                    Resetoi se tällä: {resetToken}",
                To = user.Email
            });
        }

        public async Task SendPasswordChanged(ApplicationUser user) {
            await emailSender.SendEmailAsync(new Email() {
                From = "palvelu@example.com",
                Subject = "Salasanasi on vaihdettu",
                Body = $"Joku vaihtoi salasanasi...",
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