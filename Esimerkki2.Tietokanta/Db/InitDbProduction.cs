
using System.Threading.Tasks;

namespace Esimerkki2.Tietokanta.Db
{
    public class InitDbProduction : IInitDb
    {
        public async Task InitDb()
        {
            // Voit ajaa erinäköisiä toimenpiteitä prosessin käynnistyksessä
            // tässä kohti, joissain tilanteissa esim migraatioita tms.
        }
    }
}