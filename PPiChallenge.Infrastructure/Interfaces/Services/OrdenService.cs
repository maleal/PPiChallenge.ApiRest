using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PPiChallenge.Core.DTOs;
using PPiChallenge.Core.Entities;
using PPiChallenge.Core.Interfaces;
using PPiChallenge.Core.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPiChallenge.Infrastructure.Interfaces.Services
{
    public class OrdenService : IOrdenService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<OrdenService> _logger;

        #region -- Constructor --
        public OrdenService(IUnitOfWork unitOfWork, ILogger<OrdenService> logger)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        #endregion

        public async Task<OrdenDto> CrearOrdenAsync(CrearOrdenDto dto)
        {
            try
            {
                _logger.LogInformation($"Iniciando creación de orden para CuentaId={dto.CuentaId}, Activo={dto.Ticker}");
                _logger.LogDebug($"Activo={dto.Ticker}, Cantidad={dto.Cantidad}, Operacion={dto.Operacion}");


                var cuenta = await _unitOfWork.Cuentas.GetByIdAsync(dto.CuentaId);
                if (cuenta == null)
                {
                    _logger.LogError("No se encontro la cuenta Id={CuentaId}", dto.CuentaId);
                    throw new KeyNotFoundException($"Cuenta con Id {dto.CuentaId} no encontrada.");
                }
                    
                //var activo = await _unitOfWork.ActivosFinancieros.GetByIdWithTipoActivoAsync(dto.ActivoFinancieroId);
                var activo = await _unitOfWork.ActivosFinancieros.GetByTickerWithTipoActivoAsync(dto.Ticker);
                if (activo == null)
                {
                    _logger.LogError($"No se encontró el activo financiero con codigo={dto.Ticker}");
                    throw new KeyNotFoundException($"Activo con Tickerd {dto.Ticker} no encontrado.");
                }

                //Valido tambien aqui de recibir Precio unitario para los BONOS y FCI
                if ((activo.TipoActivo.Descripcion == "Bono" || activo.TipoActivo.Descripcion == "FCI")
                     && dto.PrecioUnitario == null)
                {
                    throw new ArgumentException("Debe especificar PrecioUnitario para este tipo de activo.");
                }

                //Solo estos dos tipos de operacion
                if (dto.Operacion != "C" && dto.Operacion != "V")
                    throw new ArgumentException("La operación debe ser 'C' o 'V'.");

                //Calcular valores según tipo de activo
                decimal precioRx;
                decimal comision = 0m;
                decimal impuesto = 0m;
                decimal montoTotal;

                switch (activo.TipoActivo.Descripcion)
                {
                    case "Accion": //Accion 1
                        precioRx = activo.PrecioUnitario; // Se obtiene de la DB
                        montoTotal = precioRx * dto.Cantidad;
                        comision = montoTotal * 0.006m;
                        impuesto = comision * 0.21m;
                        montoTotal += comision + impuesto;
                        break;
                    case "Bono": //"Bono" 2
                        precioRx = dto.PrecioUnitario ?? 0m;
                        montoTotal = precioRx * dto.Cantidad;
                        comision = montoTotal * 0.002m;
                        impuesto = comision * 0.21m;
                        montoTotal += comision + impuesto;
                        break;
                    case "FCI": //FCI 3
                        precioRx = dto.PrecioUnitario ?? 0m;
                        montoTotal = precioRx * dto.Cantidad;
                        break;

                    default:
                        throw new InvalidOperationException($"Tipo de activo '{activo.TipoActivo}' no soportado.");

                }
                
                //Armamos Orden para guardar en DB
                var orden = new Orden
                {
                    IdCuenta = dto.CuentaId,
                    ActivoFinancieroId = activo.Id, //dto.ActivoFinancieroId,
                    Cantidad = dto.Cantidad,
                    Operacion = dto.Operacion,
                    Estado = EstadoDeOrden.EnProceso,
                    Precio = precioRx,
                    MontoTotal = montoTotal,
                    Comision   = comision,
                    Impuesto = impuesto
                };

                await _unitOfWork.Ordenes.AddAsync(orden);
                await _unitOfWork.SaveChangesAsync();

                return new OrdenDto
                {
                    Id = orden.IdOrden,
                    CuentaId = orden.IdCuenta,
                    ActivoFinancieroId = orden.ActivoFinancieroId,
                    Cantidad = orden.Cantidad,
                    Operacion = orden.Operacion,
                    DescripcionEstado = orden.Estado.ToString(),
                    Precio = precioRx,
                    MontoTotal = orden.MontoTotal,
                    Comision = orden.Comision,
                    Impuesto = orden.Impuesto
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al crear orden para DTO {@dto}");
                throw;
            }
            
        }

        public async Task<IEnumerable<OrdenDto>> ObtenerOrdenesPorCuentaAsync(int cuentaId)
        {
            try
            {
                _logger.LogInformation("Obtener ordenes para CuentaId={CuentaId}", cuentaId);

                var ordenes = await _unitOfWork.Ordenes.GetByCuentaIdAsync(cuentaId);

                // LogDebug con las órdenes encontradas
                var resumenOrdenes = ordenes
                    .Select(o => $"Id={o.IdOrden}, DescripcionEstado={o.Estado}, Cantidad={o.Cantidad}, Operacion={o.Operacion}")
                    .ToList();
                _logger.LogDebug("Órdenes encontradas para cuentaId={CuentaId}: {@ResumenOrdenes}", cuentaId, resumenOrdenes);

                return ordenes.Select(o => new OrdenDto
                {
                    Id = o.IdOrden,
                    CuentaId = o.IdCuenta,
                    ActivoFinancieroId = o.ActivoFinancieroId,
                    Cantidad = o.Cantidad,
                    Operacion = o.Operacion.ToString(),
                    DescripcionEstado = o.Estado.ToString()
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error obteniendo ordenes para cuentaId={cuentaId}");
                throw;
            }
        }

        public async Task<OrdenDto> ActualizarEstadoOrdenAsync(int ordenId, EstadoDeOrden nuevoEstado)
        {
            var orden = await _unitOfWork.Ordenes.GetByIdAsync(ordenId);
            if (orden == null)
                throw new KeyNotFoundException($"Orden con Id {ordenId} no encontrada.");

            orden.Estado = nuevoEstado;
            _unitOfWork.Ordenes.UpdateAsync(orden);
            await _unitOfWork.SaveChangesAsync();

            return new OrdenDto
            {
                Id = orden.IdOrden,
                CuentaId = orden.IdCuenta,
                ActivoFinancieroId = orden.ActivoFinancieroId,
                Cantidad = orden.Cantidad,
                Operacion = orden.Operacion,
                DescripcionEstado = orden.Estado.ToString()
            };
        }

        public async Task<bool> EliminarOrdenAsync(int ordenId)
        {
            //var orden = await _unitOfWork.Ordenes.GetByIdAsync(ordenId);
            //if (orden == null)
            //    return false;

            //_unitOfWork.Ordenes.DeleteAsync(orden);
            //await _unitOfWork.SaveChangesAsync();

            //return true;
            try
            {
                _logger.LogInformation($"Eliminando orden Id={ordenId}");

                var orden = await _unitOfWork.Ordenes.GetByIdAsync(ordenId);
                if (orden == null)
                {
                    _logger.LogWarning($"Orden Id={ordenId} no encontrada para eliminar");
                    return false;
                }

                _unitOfWork.Ordenes.DeleteAsync(orden);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation($"Orden Id={ordenId} eliminada correctamente");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error eliminando orden Id={ordenId}");
                throw;
            }
        }

        public async Task<OrdenDto> ObtenerOrdenPorIdAsync(int id)
        {
            try
            {
                _logger.LogInformation("Obteniendo orden Id={OrdenId}", id);

                var orden = await _unitOfWork.Ordenes.GetByIdAsync(id);
                if (orden == null)
                {
                    _logger.LogError("Orden Id={OrdenId} no encontrada", id);
                    throw new KeyNotFoundException($"Orden con Id {id} no encontrada.");
                }

                //SE PODRIA RETORNAR return MapToDto(orden); Lo hare mas adelante !!! Mario
                return new OrdenDto
                {
                    Id = orden.IdOrden,
                    CuentaId = orden.IdCuenta,
                    ActivoFinancieroId = orden.ActivoFinancieroId,
                    Cantidad = orden.Cantidad,
                    Operacion = orden.Operacion.ToString(),
                    DescripcionEstado = orden.Estado.ToString()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo orden Id={OrdenId}", id);
                throw;
            }
        }

        public async Task<IEnumerable<OrdenDto>> ObtenerTodasAsync()
        {
            try {
                _logger.LogInformation($"Obtener todas las Ordenes ya creadas");
                _logger.LogDebug($"Intento Obtener todas las Ordenes ya creadas");

                var ordenes = await _unitOfWork.Ordenes.GetAllAsync();

                // Para LogDebug!! ordenes encontradas
                var resumenOrdenes = ordenes
                    .Select(o => $"Id={o.IdOrden}, CuentaId={o.IdCuenta}, DescripcionEstado={o.Estado}, Cantidad={o.Cantidad}, Operacion={o.Operacion}")
                    .ToList();
                _logger.LogDebug($"Ordenes encontradas:{@resumenOrdenes}");

                return ordenes.Select(o => new OrdenDto
                {
                    Id = o.IdOrden,
                    CuentaId = o.IdCuenta,
                    ActivoFinancieroId = o.ActivoFinancieroId,
                    Cantidad = o.Cantidad,
                    Operacion = o.Operacion,
                    DescripcionEstado = o.Estado.ToString(),
                    Precio = o.Precio,
                    Comision =  o.Comision,
                    Impuesto = o.Impuesto,
                    MontoTotal = o.MontoTotal
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo todas las órdenes");
                throw;
            }

        }

        public async Task<OrdenDto> ActualizarEstadoOrdenAsync(int ordenId, string descripcionEstado)
        {
            try {
                _logger.LogInformation($"Actualizar DescripcionEstado Orden ya creadas");
                _logger.LogDebug($"Actualizar DescripcionEstado Orden ya creadas");

                var orden = await _unitOfWork.Ordenes.GetByIdAsync(ordenId);
                if (orden == null)
                {
                    _logger.LogError("Orden Id={OrdenId} no encontrada", ordenId);
                    throw new KeyNotFoundException($"Orden con Id {ordenId} no encontrada.");
                }

                var estado = await _unitOfWork.EstadosOrden.GetByDescripcionEstadoAsync(descripcionEstado);
                if (estado == null)
                {
                    _logger.LogError($"DescripcionEstado de Orden con descripcion={descripcionEstado} no encontrado");
                    throw new KeyNotFoundException($"DescripcionEstado de Orden con descripcion {descripcionEstado} no encontrado.");
                }

                orden.Estado = (EstadoDeOrden)estado.Id;
                _unitOfWork.Ordenes.UpdateAsync(orden);
                await _unitOfWork.SaveChangesAsync();

                //MAPEO EN LINEA (ver de usar un automaper)
                return new OrdenDto
                {
                    Id = orden.IdOrden,
                    CuentaId = orden.IdCuenta,
                    ActivoFinancieroId = orden.ActivoFinancieroId,
                    Cantidad = orden.Cantidad,
                    Operacion = orden.Operacion,
                    DescripcionEstado = estado.DescripcionEstado,  //Usamos la descripción del estado
                    Precio = orden.Precio,
                    Comision = orden.Comision,
                    Impuesto = orden.Impuesto,
                    MontoTotal = orden.MontoTotal
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error ActualizarEstadoOrdenAsync");
                throw;
            }
            //throw new NotImplementedException();
        }
    }
}
