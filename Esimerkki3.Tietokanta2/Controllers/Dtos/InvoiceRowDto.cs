using System;
using System.ComponentModel.DataAnnotations;
using Esimerkki3.Tietokanta2.Models;

namespace Esimerkki3.Tietokanta2.Controllers.Dtos
{
    public class InvoiceRowDto {
        public int? Id { get; set; }
        
        [MinLength(3)]
        public string Name { get; set; }
        public int Quantity { get; set; }
        public Decimal Amount { get; set; }

        public static InvoiceRowDto FromInvoiceRow(InvoiceRow invoiceRow) {
            return new InvoiceRowDto() {
                Id = invoiceRow.Id,
                Name = invoiceRow.Name,
                Quantity = invoiceRow.Quantity,
                Amount = invoiceRow.Amount,
            };
        }

        public InvoiceRow ToInvoiceRow(Invoice invoice) {
            return new InvoiceRow() {
                Id = Id ?? 0,
                Name = Name,
                Quantity = Quantity,
                Amount = Amount,
                Invoice = invoice
            };
        }
    }
}