using Microsoft.AspNetCore.Identity;

namespace Esimerkki4.Kirjautuminen.Models
{
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

        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
    }
}