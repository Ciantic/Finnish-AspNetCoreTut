using System;
using System.Linq;
using System.Threading.Tasks;
using Esimerkki4.Kirjautuminen.Db;
using Esimerkki4.Kirjautuminen.Models;
using Microsoft.EntityFrameworkCore;

namespace Esimerkki4.Kirjautuminen.Stores {
    public class BusinessStore
    {
        private readonly AppDbContext dbContext;

        public BusinessStore(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task Update(Business business) {
            business.Modified = DateTime.UtcNow;
            dbContext.Business.Update(business);
            await dbContext.SaveChangesAsync();
        }

        public async Task<Business> Create(Business business) {
            business.Created = DateTime.UtcNow;
            dbContext.Business.Add(business);
            await dbContext.SaveChangesAsync();
            return business;
        }

        public async Task<Business> Get(int id) {
            var value = await dbContext.Business.FirstOrDefaultAsync(t => t.Id == id);
            if (value == null) {
                throw new NotFoundException();
            }
            return value;
        }

        public async Task<Business> GetByUserId(int userId) {
            var value = await dbContext.Business.FirstOrDefaultAsync(t => t.OwnerApplicationUserId == userId);
            if (value == null) {
                throw new NotFoundException();
            }

            return value;
        }

        public async Task<bool> IsOwnerUser(int userId) {
            return await dbContext.Business
                .AnyAsync(t => t.OwnerApplicationUserId == userId);
        }
    }
}