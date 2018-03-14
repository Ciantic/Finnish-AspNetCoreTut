using System;

namespace Esimerkki4.Kirjautuminen.Models
{
    public class InvoiceRow
    {
        public int Id { get; set; }

        // Suora viittaus
        public int InvoiceId { get; set; }
        public Invoice Invoice { get; set; }

        public string Name { get; set; }
        public int Quantity { get; set; }
        public Decimal Amount { get; set; }
        public int Sort { get; set; } = 0;
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
    }
}