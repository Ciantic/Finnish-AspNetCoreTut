using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Esimerkki2.Tietokanta.Models
{
// Nämä modelit ovat tässä samassa tiedostossa jotta niitä on helpompi demota,
// oikeasti ne kannataa siirtä omiin tiedostoihinsa kuten C#:ssa on tapana

public class ApplicationUser : IdentityUser {
    // IdentityUser on ASP.NET Identity kirjaston yläluokka käyttäjäjärjestelmää
    // varten.
    //
    // Tänne periytyy propertyjä kuten Username, Email, Password, ... vaikka et
    // tarvisi kaikkia propertyjä jotka tänne periytyy, niin ne kannatta
    // säilyttää jotta ei tule päänsärkyä Identity kirjaston kanssa
    // yhteensopivuudesta, kaikki propertyjä ei ole pakko käyttää
}

public class Business {
    public int Id { get; set; }
    public string Title { get; set; }

    public string OwnerApplicationUserId { get; set; }
    public ApplicationUser OwnerApplicationUser { get; set; }
}

public class Client {
    public int Id { get; set; }
    public string Title { get; set; }
    public string Address { get; set; }
    public string City { get; set; }
    public string PostCode { get; set; }
}

public class Invoice {
    public int Id { get; set; }
    public int Title { get; set; }
    public string Info { get; set; }
    public DateTime Sent { get; set; }


    public Client Client { get; set; }
    public int ClientId { get; set; }

    // Vastakkain oleva referenssi (Inverse navigation), viittaa tämän laskun
    // InvoiceRow listaan
    public List<InvoiceRow> InvoiceRows { get; set; }

    public DateTime Created { get; set; }
    public DateTime Modified { get; set; }
}

public class InvoiceRow {
    public int Id { get; set; }
    public int InvoiceId { get; set; }

    // Suora viittaus
    public Invoice Invoice { get; set; }
    public string Name { get; set; }
    public int Quantity { get; set; }
    public Decimal Amount { get; set; }
    public DateTime Created { get; set; }
    public DateTime Modified { get; set; }
}

public class Email {
    public int Id { get; set; }
    public string To { get; set; }
    public string From { get; set; }
    public string Subject { get; set; }
    public string Body { get; set; }
}
}