using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PPiChallenge.Core.Entities;
using PPiChallenge.Core.Interfaces.Repository;
using PPiChallenge.Infrastructure.DataBaseIttion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPiChallenge.Infrastructure.Interfaces.Repository
{
    public class OrdenRepository : GenericRepository<Orden>, IOrdenRepository
    {
        public OrdenRepository(ApplicationDbContext dbContext, ILogger<GenericRepository<Orden>> logger) : base(dbContext, logger)
        {
        }

        public async Task<IEnumerable<Orden>> GetByCuentaIdAsync(int cuentaId)
        {
            return await _dbSet
                .Where(o => o.IdCuenta == cuentaId)
                .ToListAsync();
        }
    }
}
