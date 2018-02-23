using System.Threading.Tasks;
using Esimerkki3.Tietokanta2.Models;

namespace Esimerkki3.Tietokanta2.Services {
    public interface IEmailSender {
        Task<Email> SendEmailAsync(Email email);
    }
}