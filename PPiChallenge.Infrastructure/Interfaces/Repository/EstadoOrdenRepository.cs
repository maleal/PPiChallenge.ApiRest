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
    public class EstadoOrdenRepository : GenericRepository<EstadoOrden>, IEstadoOrdenRepository
    {
        public EstadoOrdenRepository(ApplicationDbContext dbContext, ILogger<EstadoOrdenRepository> logger) : base(dbContext, logger)
        {
            
        }
        public async Task<EstadoOrden> GetByDescripcionEstadoAsync(string descripcion)
        {
            return await _dbSet.FirstOrDefaultAsync(e => e.DescripcionEstado == descripcion);
        }
    }
}
