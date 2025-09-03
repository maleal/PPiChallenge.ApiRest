using PPiChallenge.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPiChallenge.Core.Interfaces.Repository
{
    public interface IActivoFinancieroRepository : IGenericRepository<ActivoFinanciero>
    {
        Task<ActivoFinanciero> GetByIdWithTipoActivoAsync(int id);
        Task<ActivoFinanciero> GetByTickerAsync(string ticker);
        Task<ActivoFinanciero> GetByTickerWithTipoActivoAsync(string ticker);
    }
}
