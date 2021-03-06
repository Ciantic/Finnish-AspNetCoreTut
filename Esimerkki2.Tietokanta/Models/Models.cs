using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;

namespace Esimerkki2.Tietokanta.Models
{
    // Nämä modelit ovat tässä samassa tiedostossa jotta niitä on helpompi demota,
    // oikeasti ne kannataa siirtä omiin tiedostoihinsa kuten C#:ssa on tapana

    public class ApplicationUser : IdentityUser<int> // Primary keyn tyyppi Int
    {
        // IdentityUser on ASP.NET Identity kirjaston yläluokka käyttäjiä
        // varten.
        //
        // Tänne periytyy propertyjä kuten Username, Email, Password, ... vaikka
        // et tarvisi kaikkia propertyjä jotka tänne periytyy, niin ne kannatta
        // säilyttää jotta ei tule päänsärkyä Identity kirjaston kanssa
        // yhteensopivuudesta, kaikkia propertyjä ei ole pakko käyttää omassa
        // toteutuksessa
    }

    public class ApplicationRole : IdentityRole<int> {
        // Käyttäjän rooli ohjelmassa, en käytä tätä tässä esimerkissä
    }

    public class Business
    {
        public int Id { get; set; }
        public string Title { get; set; }

        // Suora viittaus
        public int OwnerApplicationUserId { get; set; }
        public ApplicationUser OwnerApplicationUser { get; set; }
    }

    public class Client
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string PostCode { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }

        // Suora viittaus
        public int BusinessId { get; set; }
        public Business Business { get; set; }
    }

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

    public class InvoiceRow
    {
        public int Id { get; set; }

        // Suora viittaus
        public int InvoiceId { get; set; }

        [JsonIgnore] // Tämä estää referenssien loopin tässä esimerkissä
        public Invoice Invoice { get; set; }

        public string Name { get; set; }
        public int Quantity { get; set; }
        public Decimal Amount { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
    }

    public class Email
    {
        public int Id { get; set; }
        public string To { get; set; }
        public string From { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
    }
}