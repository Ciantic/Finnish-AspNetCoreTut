using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Esimerkki3.Tietokanta2.Models;

namespace Esimerkki3.Tietokanta2.Controllers.Dtos
{
    public class InvoiceDto {
        public int Id { get; set; }

        [MinLength(3)]
        public string Title { get; set; } = "";
        public DateTime? Sent { get; set; }

        public ClientDto Client { get; set; }
        public IList<InvoiceRowDto> InvoiceRows { get; set; } = new List<InvoiceRowDto>();
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }

        public static InvoiceDto FromInvoice(Invoice invoice) {
            // Tässä kannattaisi käyttää AutoMapper kirjastoa eikä kirjoittaa näitä käsin
            return new InvoiceDto() {
                Id = invoice.Id,
                Title = invoice.Title,
                Sent = invoice.Sent,
                Client = ClientDto.FromClient(invoice.Client),
                Created = invoice.Created,
                Modified = invoice.Modified,
                InvoiceRows = invoice.InvoiceRows
                    .Select(t => InvoiceRowDto.FromInvoiceRow(t))
                    .ToList()
            };
        }

        public Invoice UpdateInvoice(Invoice invoice) {
            invoice.Title = Title;
            invoice.ClientId = Client?.Id;
            invoice.Client = null; // Vain ID:tä vaihdetaan päivittäessä
            invoice.InvoiceRows = InvoiceRows
                .Select(t => t.ToInvoiceRow(invoice))
                .ToList();
            return invoice;
        }
    }
}