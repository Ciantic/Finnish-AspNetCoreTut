using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Esimerkki4.Kirjautuminen.Auth;
using Esimerkki4.Kirjautuminen.Controllers.Dtos;
using Esimerkki4.Kirjautuminen.Models;
using Esimerkki4.Kirjautuminen.Mvc;
using Esimerkki4.Kirjautuminen.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Esimerkki4.Kirjautuminen.Services.InvoiceService;

namespace Esimerkki4.Kirjautuminen.Controllers
{
    [Authorize]
    [Route("[controller]")]
    public class InvoicesController : ControllerBase
    {
        private readonly InvoiceService invoiceService;
        private readonly InvoicesAuthorize invoiceAuthorize;

        public InvoicesController(InvoiceService invoiceService, InvoicesAuthorize invoiceAuthorize)
        {
            this.invoiceService = invoiceService;
            this.invoiceAuthorize = invoiceAuthorize;
        }

        [HttpGet("{id}")]
        public async Task<InvoiceDto> Get(int id, [RequestBusiness] Business business) {
            if (!await invoiceAuthorize.CanReadInvoice(User, id)) {
                throw new Forbidden();
            }

            var invoice = await invoiceService.GetByBusiness(business.Id, id);
            return InvoiceDto.FromInvoice(invoice);
        }

        [HttpDelete("{id}")]
        public async Task<bool> Delete(int id, [RequestBusiness] Business business) {
            if (!await invoiceAuthorize.CanDeleteInvoice(User, id)) {
                throw new Forbidden();
            }

            var invoice = await invoiceService.GetByBusiness(business.Id, id);
            await invoiceService.Remove(invoice);
            return true;
        }

        [HttpPut("{id}")]
        public async Task<InvoiceDto> Update(
            int id, 
            [FromBody] InvoiceUpdateDto updateInvoiceDto,
            [RequestBusiness] Business business)
        {
            if (!await invoiceAuthorize.CanUpdateInvoice(User, id, updateInvoiceDto)) {
                throw new Forbidden();
            }

            var invoice = await invoiceService.GetByBusiness(business.Id, id);

            // Päivitä laskua dtosta
            updateInvoiceDto.UpdateInvoice(invoice);
            await invoiceService.Update(invoice);

            // Palauta päivitetty lasku, kierretään tietokannan kautta jotta
            // data päivittyy oikein
            return InvoiceDto.FromInvoice(
                await invoiceService.GetByBusiness(business.Id, id)
            );
        }

        [HttpPost]
        public async Task<InvoiceDto> Create([RequestBusiness] Business business) {
            if (!await invoiceAuthorize.CanCreateInvoice(User)) {
                throw new Forbidden();
            }

            // Huomaa että tässä esimerkissä en ota sisään dataa josta lasku
            // luotaisiin, sillä haluan että tätä järjestelmää käytettäessä
            // luodaan aina ensin luonnos, eli tyhjä lasku jota aletaan
            // muokkaamaan.
            var invoice = await invoiceService.Create(new Invoice() {
                BusinessId = business.Id
            });
            return InvoiceDto.FromInvoice(invoice);
        }

        [HttpGet]
        public async Task<IEnumerable<InvoiceDto>> ListLatest([RequestBusiness] Business business) {
            if (!await invoiceAuthorize.CanListInvoices(User)) {
                throw new Forbidden();
            }

            return (await invoiceService.ListLatestByBusiness(business.Id))
                .Select(t => InvoiceDto.FromInvoice(t))
                .ToList();
        }

        public class InvoiceSendError : ApiError<List<string>> {
            public InvoiceSendError(string message)
            {
                this.JsonData = new List<string> { message };
            }
        }

        [HttpPost("{id}/[action]")]
        public async Task<InvoiceDto> Send(int id, [RequestBusiness] Business business)
        {
            if (!await invoiceAuthorize.CanSendInvoice(User, id)) {
                throw new Forbidden();
            }

            var invoice = await invoiceService.GetByBusiness(business.Id, id);

            try {
                var sentInvoice = await invoiceService.Send(invoice);
                return InvoiceDto.FromInvoice(sentInvoice);
            } catch (InvoiceSendException ies) {
                throw new InvoiceSendError(ies.Message);
            }
        }
    }
}
