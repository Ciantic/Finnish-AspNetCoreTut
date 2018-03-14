using System;

namespace Esimerkki4.Kirjautuminen.Models
{

    public class Email
    {
        public int Id { get; set; }
        public string To { get; set; }
        public string From { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Sent { get; set; }
    }
}