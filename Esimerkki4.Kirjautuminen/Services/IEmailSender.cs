using System.Threading.Tasks;
using Esimerkki4.Kirjautuminen.Models;

namespace Esimerkki4.Kirjautuminen.Services {
    public interface IEmailSender {
        Task<Email> SendEmailAsync(Email email);
    }
}