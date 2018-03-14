
using System.Threading.Tasks;

namespace Esimerkki4.Kirjautuminen.Db
{
    public class InitDbProduction : IInitDb
    {
        public async Task Init()
        {
            // Voit ajaa erinäköisiä toimenpiteitä prosessin käynnistyksessä
            // tässä kohti, joissain tilanteissa esim migraatioita, tarkistuksia tms.

            // Huomattavaa on että ASP.NET Core prosesseja käynnistetään
            // tarvittaessa ja niitä on usein elossa useita samaan aikaan eli
            // toiminnot joita tuotannossa tässä kohti voi ajaa on hyvin
            // rajattuja
        }
    }
}