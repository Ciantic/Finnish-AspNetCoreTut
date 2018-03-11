using System;

namespace Esimerkki3.Tietokanta2.Models
{
    public class Client
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Address { get; set; } = "";
        public string City { get; set; } = "";
        public string PostCode { get; set; } = "";
        public string Email { get; set; } = "";
        public string PhoneNumber { get; set; } = "";

        // Suora viittaus
        public int BusinessId { get; set; }
        public Business Business { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
    }
}