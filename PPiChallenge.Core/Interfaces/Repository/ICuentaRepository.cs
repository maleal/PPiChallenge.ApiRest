using PPiChallenge.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPiChallenge.Core.Interfaces.Repository
{
    public interface ICuentaRepository :IGenericRepository<Cuenta>
    {
        Task<Cuenta> GetByEmailAsync(string email);
        Task<bool> IsEnabled(string email);
        Task<Cuenta?> GetByUsuarioAsync(string usuario);
    }
}
