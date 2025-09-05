using PPiChallenge.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPiChallenge.Core.Interfaces.Repository
{
    public interface IEstadoOrdenRepository : IGenericRepository<EstadoOrden>
    {
        Task<EstadoOrden> GetByDescripcionEstadoAsync(string descripcion);
    }
}
