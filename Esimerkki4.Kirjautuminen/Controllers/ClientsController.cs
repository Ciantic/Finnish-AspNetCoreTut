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

namespace Esimerkki4.Kirjautuminen.Controllers
{
    [Authorize]
    [Route("[controller]")]
    public class ClientsController : ControllerBase
    {
        private readonly ClientService clientService;
        private readonly ClientsAuthorize clientsAuthorize;

        public ClientsController(ClientService clientService, ClientsAuthorize clientsAuthorize)
        {
            this.clientService = clientService;
            this.clientsAuthorize = clientsAuthorize;
        }

        [HttpGet("{id}")] 
        public async Task<ClientDto> Get(int id, [RequestBusiness] Business business) {
            if (!await clientsAuthorize.CanReadClient(User, id)) {
                throw new Forbidden();
            }

            var client = await clientService.GetByBusiness(business.Id, id);
            return ClientDto.FromClient(client);
        }

        [HttpDelete("{id}")]
        public async Task<bool> Delete(int id, [RequestBusiness] Business business) {
            if (!await clientsAuthorize.CanDeleteClient(User, id)) {
                throw new Forbidden();
            }

            var client = await clientService.GetByBusiness(business.Id, id);
            await clientService.Remove(client);
            return true;
        }

        [HttpPut("{id}")]
        public async Task<ClientDto> Update(int id, [FromBody] ClientDto clientDto, 
            [RequestBusiness] Business business)
        {
            if (!await clientsAuthorize.CanUpdateClient(User, id, clientDto)) {
                throw new Forbidden();
            }

            var client = await clientService.GetByBusiness(business.Id, id);

            // Päivitä laskua dtosta
            clientDto.UpdateClient(client);
            await clientService.Update(client);

            // Palauta päivitetty lasku
            return ClientDto.FromClient(client);
        }

        [HttpPost]
        public async Task<ClientDto> Create([RequestBusiness] Business business) {
            if (!await clientsAuthorize.CanCreateClient(User)) {
                throw new Forbidden();
            }

            var client = await clientService.Create(new Client() {
                BusinessId = business.Id
            });
            return ClientDto.FromClient(client);
        }

        [HttpGet]
        public async Task<IEnumerable<ClientDto>> List([RequestBusiness] Business business) {
            if (!await clientsAuthorize.CanListClients(User)) {
                throw new Forbidden();
            }

            return (await clientService.ListByBusiness(business.Id))
                .Select(t => ClientDto.FromClient(t))
                .ToList();
        }
    }
}
