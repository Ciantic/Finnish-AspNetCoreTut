using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Esimerkki4.Kirjautuminen.Controllers.Dtos;
using Esimerkki4.Kirjautuminen.Models;
using Esimerkki4.Kirjautuminen.Services;
using Microsoft.AspNetCore.Mvc;

namespace Esimerkki4.Kirjautuminen.Controllers
{
    // Tämä määrittelee alkuosaksi "api/values"
    [Route("[controller]")]
    public class ClientsController
    {
        private readonly ClientService clientService;

        public ClientsController(ClientService clientService)
        {
            this.clientService = clientService;
        }

        [HttpGet("{id}")] 
        public async Task<ClientDto> Get(int id) {
            // Tässä esimerkissä businessId on vakio, seuraavassa esimerkissä se
            // haetaan käyttäjän tiedoista
            var businessId = 1;
            var client = await clientService.GetByBusiness(businessId, id);
            return ClientDto.FromClient(client);
        }

        [HttpDelete("{id}")]
        public async Task<bool> Delete(int id) {
            // Tässä esimerkissä businessId on vakio, seuraavassa esimerkissä se
            // haetaan käyttäjän tiedoista
            var businessId = 1;
            var client = await clientService.GetByBusiness(businessId, id);
            await clientService.Remove(client);
            return true;
        }

        [HttpPut("{id}")]
        public async Task<ClientDto> Update(int id, [FromBody] ClientDto clientDto) {
            // Tässä esimerkissä businessId on vakio, seuraavassa esimerkissä se
            // haetaan käyttäjän tiedoista
            var businessId = 1;
            var client = await clientService.GetByBusiness(businessId, clientDto.Id);

            // Päivitä laskua dtosta
            clientDto.UpdateClient(client);
            await clientService.Update(client);

            // Palauta päivitetty lasku
            return ClientDto.FromClient(client);
        }

        [HttpPost]
        public async Task<ClientDto> Create() {
            // Huomaa että tässä esimerkissä en ota sisään dataa josta lasku
            // luotaisiin, sillä haluan että tätä järjestelmää käytettäessä
            // luodaan aina ensin luonnos, eli tyhjä lasku jota aletaan
            // muokkaamaan.

            // Tässä esimerkissä businessId on vakio, seuraavassa esimerkissä se
            // haetaan käyttäjän tiedoista
            var businessId = 1;

            var client = await clientService.Create(new Client() {
                BusinessId = businessId
            });
            return ClientDto.FromClient(client);
        }

        [HttpGet]
        public async Task<IEnumerable<ClientDto>> List() {
            // Tässä esimerkissä businessId on vakio, seuraavassa esimerkissä se
            // haetaan käyttäjän tiedoista
            var businessId = 1;
            return (await clientService.ListByBusiness(businessId))
                .Select(t => ClientDto.FromClient(t))
                .ToList();
        }
    }
}
