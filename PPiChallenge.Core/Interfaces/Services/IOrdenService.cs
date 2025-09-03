using PPiChallenge.Core.DTOs;
using PPiChallenge.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPiChallenge.Core.Interfaces.Services
{
    public interface IOrdenService
    {
        Task<IEnumerable<OrdenDto>> ObtenerTodasAsync();
        Task<OrdenDto> CrearOrdenAsync(CrearOrdenDto dto);
        Task<OrdenDto> ObtenerOrdenPorIdAsync(int id);
        Task<IEnumerable<OrdenDto>> ObtenerOrdenesPorCuentaAsync(int cuentaId);
        Task<OrdenDto> ActualizarEstadoOrdenAsync(int ordenId, EstadoDeOrden nuevoEstado);
        Task<bool> EliminarOrdenAsync(int ordenId);
    }
}
