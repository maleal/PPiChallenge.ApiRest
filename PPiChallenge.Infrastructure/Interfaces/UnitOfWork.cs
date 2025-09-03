using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PPiChallenge.Core.Entities;
using PPiChallenge.Core.Interfaces;
using PPiChallenge.Core.Interfaces.Repository;
using PPiChallenge.Infrastructure.DataBaseIttion;
using PPiChallenge.Infrastructure.Interfaces.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPiChallenge.Infrastructure.Interfaces
{
    public class UnitOfWork : IUnitOfWork
    {
        public IActivoFinancieroRepository ActivosFinancieros { get; }
        public ICuentaRepository Cuentas { get; }
        public IOrdenRepository Ordenes { get; }
        private readonly ILogger<UnitOfWork> _logger;
        private readonly ApplicationDbContext _dbContext;
        
        #region -- Constructor --
        public UnitOfWork(
                ApplicationDbContext dbContext,
                IActivoFinancieroRepository activoRepository,
                ICuentaRepository cuentaRepository,
                IOrdenRepository ordenRepository,
                ILogger<UnitOfWork> logger)
        {
            _dbContext = dbContext;
            ActivosFinancieros = activoRepository;
            Cuentas = cuentaRepository;
            Ordenes = ordenRepository;
            _logger = logger;// = logger.CreateLogger("Logs");
        }
        #endregion

        

        #region -- IDisposable --
        public void Dispose()
        {
            if (_dbContext != null)
            {
                _dbContext.Dispose();
            }
        }
        #endregion
        
        public async Task<int> SaveChangesAsync()
        {
            try
            {
                return await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al guardar cambios en la DB.");
                throw;
            }
        }
    }
}
