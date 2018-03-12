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
        private readonly ClientService clientService;

        public InvoicesController(InvoiceService invoiceService, ClientService clientService)
        {
            this.invoiceService = invoiceService;
            this.clientService = clientService;
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
        public async Task<InvoiceDto> Update(int id, [FromBody] UpdateInvoiceDto updateInvoiceDto) {
            // Tässä esimerkissä businessId on vakio, seuraavassa esimerkissä se
            // haetaan käyttäjän tiedoista
            var businessId = 1;
            var invoice = await invoiceService.GetByBusiness(businessId, id);

            // Tästä puuttuu oikeustarkistus että client Id on tämän yrityksen
            // asiakkaan ID. Sekä tarkistus että laskurivien ID:t eivät vaihdu
            // laskulta toiselle. Oikeustarkistukset tehdään seuraavassa
            // esimerkissä

            // Päivitä laskua dtosta
            updateInvoiceDto.UpdateInvoice(invoice);
            await invoiceService.Update(invoice);

            // Palauta päivitetty lasku, kierretään tietokannan kautta jotta
            // data päivittyy oikein
            return InvoiceDto.FromInvoice(
                await invoiceService.GetByBusiness(businessId, id)
            );
        }

        [HttpPost]
        public async Task<InvoiceDto> Create() {
            // Huomaa että tässä esimerkissä en ota sisään dataa josta lasku
            // luotaisiin, sillä haluan että tätä järjestelmää käytettäessä
            // luodaan aina ensin luonnos, eli tyhjä lasku jota aletaan
            // muokkaamaan.

            // Tässä esimerkissä businessId on vakio, seuraavassa esimerkissä se
            // haetaan käyttäjän tiedoista
            var businessId = 1;

            var invoice = await invoiceService.Create(new Invoice() {
                BusinessId = businessId
            });
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
