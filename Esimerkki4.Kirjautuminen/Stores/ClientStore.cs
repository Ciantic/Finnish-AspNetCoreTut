using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Esimerkki4.Kirjautuminen.Db;
using Esimerkki4.Kirjautuminen.Models;
using Microsoft.EntityFrameworkCore;

namespace Esimerkki4.Kirjautuminen.Stores {
    public class ClientStore
    {
        private readonly AppDbContext dbContext;

        public ClientStore(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task Update(Client client) {
            client.Modified = DateTime.UtcNow;
            dbContext.Client.Update(client);
            await dbContext.SaveChangesAsync();
        }

        public async Task<Client> Create(Client client) {
            client.Created = DateTime.UtcNow;
            dbContext.Client.Add(client);
            await dbContext.SaveChangesAsync();
            return client;
        }

        public async Task<Client> Remove(Client client) {
            dbContext.Client.Remove(client);
            await dbContext.SaveChangesAsync();
            return client;
        }

        public async Task<Client> GetByBusiness(int businessId, int id) {
            var value = await dbContext.Client
                .FirstOrDefaultAsync(t => t.Id == id && t.BusinessId == businessId);
            if (value == null) {
                throw new NotFoundException();
            }
            return value;
        }

        public async Task<ICollection<Client>> ListByBusiness(int businessId) {
            return await dbContext.Client
                .Where(t => t.BusinessId == businessId)
                .OrderBy(t => t.Name)
                .ToListAsync();
        }

        public async Task<bool> OwnedByBusiness(int businessId, int clientId) {
            return await dbContext.Client
                .AnyAsync(t => t.BusinessId == businessId && t.Id == clientId);
        }
    }
}