using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPiChallenge.Core.Interfaces.Repository
{
    /// <summary>
    /// Notas:
    /// Estos metodos son parte del contrato genérico base (IRepository<T>) 
    /// para tener operaciones mínimas reutilizables.
    /// </summary>
    /// <typeparam name="T"></typeparam>

    public interface IGenericRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetByIdAsync(int id);
        Task<bool> AddAsync(T entity);
        void DeleteAsync(T entity);
        void UpdateAsync(T entity);
    }
}
