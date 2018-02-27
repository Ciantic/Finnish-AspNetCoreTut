using System.Threading.Tasks;
using Esimerkki3.Tietokanta2.Db;
using Esimerkki3.Tietokanta2.Models;
using Esimerkki3.Tietokanta2.Stores;

namespace Esimerkki3.Tietokanta2.Services {
    public class EmailSender : IEmailSender {
        private readonly EmailStore emailStore;

        public EmailSender(EmailStore emailStore)
        {
            this.emailStore = emailStore;
        }

        public async Task<Email> SendEmailAsync(Email email) {
            // Tässä voisi lähettää sähköpostin oikeasti ... tai sitten sen voi
            // laittaa tietokantatauluun ja antaa toisen prosessin purkaa sitä
            // sähköpostijonoa taulussa jolloin hetkelliset yhteysvirheet tms.
            // ei hidasta tämän palvelimen toimintaa
            
            return await emailStore.Create(email);
        }
    }
}