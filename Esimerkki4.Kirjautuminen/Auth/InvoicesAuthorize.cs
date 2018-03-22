using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Esimerkki4.Kirjautuminen.Controllers.Dtos;
using Esimerkki4.Kirjautuminen.Db;
using Esimerkki4.Kirjautuminen.Models;
using Esimerkki4.Kirjautuminen.Mvc;
using Esimerkki4.Kirjautuminen.Stores;
using IdentityModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Esimerkki4.Kirjautuminen.Services {

    public class InvoicesAuthorize
    {
        private readonly BusinessAuthorize businessAuthorize;
        private readonly InvoiceStore invoiceStore;
        private readonly ClientsAuthorize clientsAuthorize;

        public InvoicesAuthorize(BusinessAuthorize businessAuthorize, InvoiceStore invoiceStore, ClientsAuthorize clientsAuthorize)
        {
            this.businessAuthorize = businessAuthorize;
            this.invoiceStore = invoiceStore;
            this.clientsAuthorize = clientsAuthorize;
        }

        public async Task<bool> CanUpdateInvoice(ClaimsPrincipal claims, int id, InvoiceUpdateDto invoiceUpdateDto)
        {
            var businessId = await businessAuthorize.GetBusinessId(claims);

            // Tarkista että laskun omistaa oikea käyttäjä
            if (!await invoiceStore.OwnedByBusiness(businessId, id)) {
                return false;
            }

            // Tarkista että olemassaolevien laskurivien ID:t kuuluvat laskulle
            if (!await invoiceStore.InvoiceRowsBelongToInvoice(id,
                invoiceUpdateDto.InvoiceRows
                    .Where(t => t.Id != null)
                    .Select(t => t.Id)
                    .Cast<int>()))
            {
                return false;
            }

            // Tarkista että asiakkaan ID on yrityksen asiakkaan ID
            if (invoiceUpdateDto.ClientId != null && 
                !await clientsAuthorize.CanReadClient(claims, (int) invoiceUpdateDto.ClientId))
            {
                return false;
            }

            return true;
        }

        public async Task<bool> CanDeleteInvoice(ClaimsPrincipal claims, int id) {
            return await IsInvoiceOwner(claims, id);
        }

        public async Task<bool> CanCreateInvoice(ClaimsPrincipal claims) {
            return await businessAuthorize.IsBusinessOwner(claims);
        }

        public async Task<bool> CanReadInvoice(ClaimsPrincipal claims, int id) {
            return await IsInvoiceOwner(claims, id);
        }

        public async Task<bool> CanListInvoices(ClaimsPrincipal claims) {
            return await businessAuthorize.IsBusinessOwner(claims);
        }

        public async Task<bool> CanSendInvoice(ClaimsPrincipal claims, int id) {
            return await IsInvoiceOwner(claims, id);
        }

        private async Task<bool> IsInvoiceOwner(ClaimsPrincipal claims, int id) {
            var businessId = await businessAuthorize.GetBusinessId(claims);

            // Tarkista että olet laskun omistaja
            if (!await invoiceStore.OwnedByBusiness(businessId, id)) {
                return false;
            }

            return true;
        }
    }
}