using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Esimerkki3.Tietokanta2.Controllers.Dtos;
using Esimerkki3.Tietokanta2.Models;
using Esimerkki3.Tietokanta2.Services;
using Microsoft.AspNetCore.Mvc;

namespace Esimerkki3.Tietokanta2.Controllers
{
    // Tämä määrittelee alkuosaksi "api/values"
    [Route("[controller]")]
    public class InvoicesController
    {
        private readonly InvoiceService invoiceService;

        public InvoicesController(InvoiceService invoiceService)
        {
            this.invoiceService = invoiceService;
        }

        [HttpGet("{id}")] 
        public async Task<InvoiceDto> Get(int id) {
            // Tässä esimerkissä businessId on vakio, seuraavassa esimerkissä se
            // haetaan käyttäjän tiedoista
            var businessId = 1;
            var invoice = await invoiceService.GetByBusiness(businessId, id);
            return InvoiceDto.FromInvoice(invoice);
        }

        [HttpDelete("{id}")]
        public async Task<bool> Delete(int id) {
            // Tässä esimerkissä businessId on vakio, seuraavassa esimerkissä se
            // haetaan käyttäjän tiedoista
            var businessId = 1;
            var invoice = await invoiceService.GetByBusiness(businessId, id);
            await invoiceService.Remove(invoice);
            return true;
        }

        [HttpPut("{id}")]
        public async Task<InvoiceDto> Update(int id, [FromBody] InvoiceDto invoiceDto) {
            // Tässä esimerkissä businessId on vakio, seuraavassa esimerkissä se
            // haetaan käyttäjän tiedoista
            var businessId = 1;
            var invoice = await invoiceService.GetByBusiness(businessId, invoiceDto.Id);

            // Päivitä laskua dtosta
            invoiceDto.ToInvoice(invoice);
            await invoiceService.Update(invoice);

            // Palauta päivitetty lasku
            return InvoiceDto.FromInvoice(invoice);
        }

        [HttpGet]
        public async Task<IEnumerable<InvoiceDto>> ListLatest() {
            // Tässä esimerkissä businessId on vakio, seuraavassa esimerkissä se
            // haetaan käyttäjän tiedoista
            var businessId = 1;
            return (await invoiceService.ListLatestByBusiness(businessId))
                .Select(t => InvoiceDto.FromInvoice(t))
                .ToList();
        }


        [HttpPost("[action]/{id}")]
        public async Task<InvoiceDto> Send(int id) {
            // Tässä esimerkissä businessId on vakio, seuraavassa esimerkissä se
            // haetaan käyttäjän tiedoista
            var businessId = 1;
            var invoice = await invoiceService.GetByBusiness(businessId, id);
            var sentInvoice = await invoiceService.Send(invoice);
            return InvoiceDto.FromInvoice(sentInvoice);
        }
    }
}
