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

    public class ClientsAuthorize
    {
        private readonly BusinessAuthorize businessAuthorize;
        private readonly ClientStore clientStore;

        public ClientsAuthorize(BusinessAuthorize businessAuthorize, ClientStore clientStore)
        {
            this.businessAuthorize = businessAuthorize;
            this.clientStore = clientStore;
        }

        public async Task<bool> CanUpdateClient(ClaimsPrincipal claims, int id, ClientDto clientDto)
        {
            var businessId = await businessAuthorize.GetBusinessId(claims);
            return await clientStore.OwnedByBusiness(businessId, id);
        }

        public async Task<bool> CanDeleteClient(ClaimsPrincipal claims, int id) {
            var businessId = await businessAuthorize.GetBusinessId(claims);
            return await clientStore.OwnedByBusiness(businessId, id);
        }

        public async Task<bool> CanCreateClient(ClaimsPrincipal claims) {
            return await businessAuthorize.IsBusinessOwner(claims);
        }

        public async Task<bool> CanReadClient(ClaimsPrincipal claims, int id) {
            var businessId = await businessAuthorize.GetBusinessId(claims);
            return await clientStore.OwnedByBusiness(businessId, id);

        }

        public async Task<bool> CanListClients(ClaimsPrincipal claims) {
            return await businessAuthorize.IsBusinessOwner(claims);
        }
    }
}