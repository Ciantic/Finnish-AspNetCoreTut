using System;

namespace Esimerkki4.Kirjautuminen.Models
{
    public class Business
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";

        // Suora viittaus
        public int OwnerApplicationUserId { get; set; }
        public ApplicationUser OwnerApplicationUser { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
    }
}