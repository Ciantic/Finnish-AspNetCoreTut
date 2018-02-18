
using System.Threading.Tasks;

namespace Esimerkki3.Tietokanta2.Db
{
    public class InitDbProduction : IInitDb
    {
        public async Task Init()
        {
            // Voit ajaa erinäköisiä toimenpiteitä prosessin käynnistyksessä
            // tässä kohti, joissain tilanteissa esim migraatioita tms.
        }
    }
}