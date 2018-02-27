using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Esimerkki3.Tietokanta2.Db;
using Esimerkki3.Tietokanta2.Models;
using Esimerkki3.Tietokanta2.Stores;
using Microsoft.EntityFrameworkCore;

namespace Esimerkki3.Tietokanta2.Services {
    public class ClientService
    {
        private readonly ClientStore clientStore;

        public ClientService(ClientStore clientStore)
        {
            this.clientStore = clientStore;
        }

        public async Task Update(Client client) {
            await clientStore.Update(client);
        }

        public async Task<Client> Create(Client client) {
            return await clientStore.Create(client);
        }

        public async Task Remove(Client client) {
            await clientStore.Remove(client);
        }

        public async Task<Client> GetByBusiness(int businessId, int id) {
            return await clientStore.GetByBusiness(businessId, id);
        }

        public async Task<ICollection<Client>> ListByBusiness(int businessId) {
            return await clientStore.ListByBusiness(businessId);
        }
    }
}