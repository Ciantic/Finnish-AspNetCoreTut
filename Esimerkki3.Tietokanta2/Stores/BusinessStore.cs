using System.Linq;
using System.Threading.Tasks;
using Esimerkki3.Tietokanta2.Db;
using Esimerkki3.Tietokanta2.Models;
using Microsoft.EntityFrameworkCore;

namespace Esimerkki3.Tietokanta2.Stores {
    public class BusinessStore
    {
        private readonly AppDbContext dbContext;

        public BusinessStore(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task Update(Business business) {
            dbContext.Business.Update(business);
            await dbContext.SaveChangesAsync();
        }

        public async Task<Business> Create(Business business) {
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
    }
}