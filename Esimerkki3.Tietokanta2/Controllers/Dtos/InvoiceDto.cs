using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Esimerkki3.Tietokanta2.Models;

namespace Esimerkki3.Tietokanta2.Controllers.Dtos
{
    public class InvoiceDto {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime? Sent { get; set; }

        [Required]
        public ClientDto Client { get; set; }
        public IList<InvoiceRowDto> InvoiceRows { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }

        public static InvoiceDto FromInvoice(Invoice invoice) {
            // Tässä kannattaisi käyttää AutoMapper kirjastoa eikä kirjoittaa näitä käsin
            return new InvoiceDto() {
                Id = invoice.Id,
                Title = invoice.Title,
                Sent = invoice.Sent,
                Created = invoice.Created,
                Modified = invoice.Modified
            };
        }

        public Invoice ToInvoice(Invoice invoice) {
            invoice.Title = Title;
            invoice.ClientId = Client.Id;
            invoice.Client = null; // Vain ID:tä vaihdetaan päivittäessä
            invoice.InvoiceRows = InvoiceRows.Select(t => t.ToInvoiceRow(invoice)).ToList();
            return invoice;
        }
    }
}