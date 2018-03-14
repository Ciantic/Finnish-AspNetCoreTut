using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Esimerkki4.Kirjautuminen.Models;

namespace Esimerkki4.Kirjautuminen.Controllers.Dtos
{
    public class InvoiceUpdateDto {

        [MinLength(3)]
        public string Title { get; set; } = "";

        public int? ClientId { get; set; }
        public List<InvoiceRowDto> InvoiceRows { get; set; } = new List<InvoiceRowDto>();

        public Invoice UpdateInvoice(Invoice invoice) {
            invoice.Title = Title;
            invoice.ClientId = ClientId;
            
            // Korvaa laskurivit uusilla
            var oldRows = invoice.InvoiceRows;
            invoice.InvoiceRows = InvoiceRows.Select((updatedRowDto, i) => {
                var invoiceRow = 
                    oldRows.Where(f => f.Id == updatedRowDto.Id).FirstOrDefault()
                    ?? new InvoiceRow() {
                        Invoice = invoice
                    };
                invoiceRow.Sort = i;
                return updatedRowDto.UpdateInvoiceRow(invoiceRow);
            }).ToList();

            return invoice;
        }
    }
}