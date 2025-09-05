using PPiChallenge.Core.Interfaces.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPiChallenge.Core.Interfaces
{
    /// <summary>
    ///     El patrón Unit of Work (UoW) coordina el trabajo de múltiples repositorios bajo una misma
    ///     transacción y contexto de base de datos. Su función principal es:
    ///         Agrupar cambios en entidades y asegurarse de que se apliquen todos juntos(o ninguno), 
    ///         a través de una única llamada a SaveChangesAsync().
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        IActivoFinancieroRepository ActivosFinancieros { get; }
        ICuentaRepository Cuentas { get; }
        IOrdenRepository Ordenes { get; }
        IEstadoOrdenRepository EstadosOrden { get; }

        Task<int> SaveChangesAsync();
        //Task<int> CompleteAsync();
    }
}
