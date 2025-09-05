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
    public class CuentaRepository : GenericRepository<Cuenta>, ICuentaRepository
    {
        public CuentaRepository(ApplicationDbContext dbContext, ILogger<CuentaRepository> logger)
        : base(dbContext, logger)
        {
        }

        public async Task<Cuenta> GetByEmailAsync(string email)
        {
            return await _dbSet.FirstOrDefaultAsync(c => c.Email == email);
        }

        public async Task<Cuenta?> GetByUsuarioAsync(string usuario)
        {
            return await _dbSet.FirstOrDefaultAsync(c => c.Usuario == usuario);
        }

        public async Task<bool> IsEnabled(string email)
        {
            try
            {
                Cuenta result = await _dbSet.AsNoTracking().FirstOrDefaultAsync(c => c.Email == email);
                
                if (result != null)
                {
                    return result.IsEnabled;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                $"Error getting Usuario: {ex.Message}");
                throw new Exception($"Error: IsEnabled()");
            }
            throw new NotImplementedException();
        }
    }
}
