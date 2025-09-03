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
    public class ActivoFinancieroRepository : GenericRepository<ActivoFinanciero>, IActivoFinancieroRepository
    {
        public ActivoFinancieroRepository(ApplicationDbContext dbContext, ILogger<GenericRepository<ActivoFinanciero>> logger)
            : base(dbContext, logger)
        {
        }

        public async Task<ActivoFinanciero?> GetByIdWithTipoActivoAsync(int id)
        {
            return await _dbSet.Include(a => a.TipoActivo).FirstOrDefaultAsync(a => a.Id == id);
            //return await _dbSet.Include(a => a.TipoActivo).Where(a => a.Id == id).FirstOrDefaultAsync();
            //throw new NotImplementedException();
        }

        public async Task<ActivoFinanciero?> GetByTickerAsync(string ticker)
        {
            return await _dbSet.FirstOrDefaultAsync(a => a.Ticker == ticker);
        }

        public async Task<ActivoFinanciero?> GetByTickerWithTipoActivoAsync(string ticker)
        {
            return await _dbSet.Include(a => a.TipoActivo).FirstOrDefaultAsync(a => a.Ticker == ticker);
        }
    }
}
