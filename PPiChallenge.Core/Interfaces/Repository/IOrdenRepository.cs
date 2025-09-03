using PPiChallenge.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPiChallenge.Core.Interfaces.Repository
{
    public interface IOrdenRepository : IGenericRepository<Orden>
    {
        Task<IEnumerable<Orden>> GetByCuentaIdAsync(int cuentaId);
    }
}
