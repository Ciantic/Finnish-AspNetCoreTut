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

    public class BusinessAuthorize
    {
        private readonly BusinessStore businessStore;
        private readonly InvoiceStore invoiceStore;
        private readonly ClientStore clientStore;

        public BusinessAuthorize(BusinessStore businessStore, InvoiceStore invoiceStore, ClientStore clientStore)
        {
            this.businessStore = businessStore;
            this.invoiceStore = invoiceStore;
            this.clientStore = clientStore;
        }

        public async Task<Business> RequestBusiness(ClaimsPrincipal claims) {
            // Tämä hakeee kirjautuneen käyttäjän yritysolion, tai heittää
            // NotFoundExceptionin
            return await businessStore.GetByUserId(GetUserId(claims));
        }

        public async Task<bool> IsBusinessOwner(ClaimsPrincipal claims) {
            // Tarkista että käyttäjä on jonkin yrityksen omistaja
            var userId = GetUserId(claims);
            return await businessStore.IsOwnerUser(userId);
        }

        public async Task<int> GetBusinessId(ClaimsPrincipal claims) {            
            // Hae käyttäjän yrityksen ID
            //
            // Tätä varten yritys ID kannattaisi sisällyttää vaateeksi
            // ClaimsPrincipaliin jotta se tulisi jo JWT tokenissa mukana,
            // tällöin sitä ei tarvisi hakea tietokannasta
            var userId = GetUserId(claims);
            return (await businessStore.GetByUserId(userId)).Id;
        }

        private int GetUserId(ClaimsPrincipal claims) {
            // Hakee JWT tokenista subject kentän, eli käyttäjän ID:n
            var val = claims?.FindFirst(JwtClaimTypes.Subject);
            var userId = 0;
            if (val != null && int.TryParse(val.Value, out userId))
            {
                return userId;
            }
            throw new Forbidden();
        }
    }
}