using System;
using System.Collections.Generic;

namespace Esimerkki3.Tietokanta2.Models
{
    public class Invoice
    {
        public int Id { get; set; }
        public string Title { get; set; }

        // Sent on oikeasti huono idea laskutusohjelmalle, lähetetty lasku pitäisi
        // viedä esim. uuteen tauluun ja arvot jäädyttää, mutta tämä on esimerkki
        public DateTime? Sent { get; set; }

        // Suora viittaus (mutta nullable huomaa "?")
        public int? ClientId { get; set; }
        public Client Client { get; set; }

        // Suora viittaus
        public int BusinessId { get; set; }
        public Business Business { get; set; }

        // Väärinpäin oleva navigointi (Inverse navigation), viittaa tämän
        // laskun InvoiceRow listaan
        public IList<InvoiceRow> InvoiceRows { get; set; }

        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
    }
}