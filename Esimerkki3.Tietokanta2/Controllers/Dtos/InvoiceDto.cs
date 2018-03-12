using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Esimerkki3.Tietokanta2.Models;

namespace Esimerkki3.Tietokanta2.Controllers.Dtos
{
    public class InvoiceDto {
        public int Id { get; set; }

        public string Title { get; set; } = "";
        public DateTime? Sent { get; set; }

        public int? ClientId { get; set; }
        public ClientDto Client { get; set; }
        public List<InvoiceRowDto> InvoiceRows { get; set; } = new List<InvoiceRowDto>();
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }

        public static InvoiceDto FromInvoice(Invoice invoice) {
            // Tässä kannattaisi käyttää AutoMapper kirjastoa eikä kirjoittaa näitä käsin
            return new InvoiceDto() {
                Id = invoice.Id,
                Title = invoice.Title,
                Sent = invoice.Sent,
                ClientId = invoice.ClientId,
                Client = ClientDto.FromClient(invoice.Client),
                Created = invoice.Created,
                Modified = invoice.Modified,
                InvoiceRows = invoice.InvoiceRows
                    .Select(t => InvoiceRowDto.FromInvoiceRow(t))
                    .ToList()
            };
        }

    }
}