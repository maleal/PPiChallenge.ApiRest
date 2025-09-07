using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PPiChallenge.Core.DTOs;
using PPiChallenge.Core.Entities;
using PPiChallenge.Core.Interfaces.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PPiChallenge.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdenController : ControllerBase
    {
        private readonly IOrdenService _ordenService;

        public OrdenController(IOrdenService ordenService)
        {
            _ordenService = ordenService ?? throw new ArgumentNullException(nameof(ordenService));
        }

        /// <summary>
        /// Obtiene todas las órdenes del sistema.
        /// </summary>
        /// <returns>Lista de todas las órdenes.</returns>
        [HttpGet]
        [Authorize]
        [ProducesResponseType(typeof(IEnumerable<OrdenDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<OrdenDto>>> ObtenerTodasOrdenes()
        {
            var ordenes = await _ordenService.ObtenerTodasAsync();
            return Ok(ordenes);
        }

        /// POST: api/orden
        /// <summary>
        /// Crea una nueva orden para un activo financiero.
        /// </summary>
        /// <remarks>
        /// Este endpoint permite crear órdenes de compra o venta de distintos tipos de activos financieros 
        /// (Acciones, Bonos, FCI) usando el <c>Ticker</c> del activo como referencia. 
        /// 
        /// Las reglas de cálculo dependen del tipo de activo:
        /// 
        /// - **Acción**: Se obtiene el precio de la base de datos. Se aplican comisiones e impuestos:
        ///   - Comisión: 0.6% del monto total.
        ///   - Impuesto: 21% sobre la comisión.
        /// 
        /// - **Bono**: El precio unitario debe ser proporcionado en el request. Se aplican comisiones e impuestos:
        ///   - Comisión: 0.2% del monto total.
        ///   - Impuesto: 21% sobre la comisión.
        /// 
        /// - **FCI**: El precio unitario debe ser proporcionado en el request. No se aplican comisiones ni impuestos.
        /// 
        /// ### Ejemplo de request:
        /// 
        ///     POST /api/orden
        ///     {
        ///        "cuentaId": 1,
        ///        "ticker": "AL30",
        ///        "precioUnitario": 555555.0555,
        ///        "cantidad": 1000,
        ///        "operacion": "C"
        ///     }
        /// 
        /// ### Ejemplo de response (OrdenDto):
        /// 
        ///     201 Created
        ///     {
        ///         "id": 123,
        ///         "cuentaId": 1,
        ///         "ticker": "AAPL",
        ///         "cantidad": 100,
        ///         "operacion": "C",
        ///         "estado": "EnProceso",
        ///         "precio": 555555.0555,
        ///         "comision": 1111110.111,
        ///         "impuesto": 233333.12331,
        ///         "montoTotal": 556899498.73431
        ///     }
        /// </remarks>
        /// <param name="dto">DTO que contiene los datos de la orden a crear.</param>
        /// <returns>Devuelve un <see cref="OrdenDto"/> con los datos de la orden creada, incluyendo montos calculados, comisiones e impuestos si corresponden.</returns>
        /// <response code="201">Orden creada correctamente. Devuelve la orden creada en formato <see cref="OrdenDto"/>.</response>
        /// <response code="400">Datos inválidos. Puede deberse a operación no permitida o falta de precio unitario en bonos o FCI.</response>
        /// <response code="404">Cuenta o activo financiero no encontrado.</response>
        [HttpPost]
        [Authorize]
        [ProducesResponseType(typeof(OrdenDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<OrdenDto>> CrearOrden([FromBody] CrearOrdenDto dto)
        {
            var ordenCreada = await _ordenService.CrearOrdenAsync(dto);
            return CreatedAtAction(nameof(ObtenerOrdenesPorCuenta), new { cuentaId = ordenCreada.CuentaId }, ordenCreada);
        }

        /// GET: api/orden/porCuenta/{cuentaId}
        /// <summary>
        /// Obtiene las Oredenes por el Id de la Cuenta.
        /// </summary>
        /// <param name="cuentaId">Id de la cuenta.</param>
        /// <returns>Lista de órdenes de la cuenta.</returns>
        [HttpGet("porCuenta/{cuentaId}")]
        [ProducesResponseType(typeof(IEnumerable<OrdenDto>), StatusCodes.Status200OK)]
        [Authorize]
        public async Task<ActionResult<IEnumerable<OrdenDto>>> ObtenerOrdenesPorCuenta(int cuentaId)
        {
            var ordenes = await _ordenService.ObtenerOrdenesPorCuentaAsync(cuentaId);
            return Ok(ordenes);
        }

        /// PATCH: api/orden/{ordenId}/estado
        /// <summary>
        /// Actualiza el estado de una orden.
        /// </summary>
        /// <remarks>
        /// Valores posibles para el estado: EnProceso, Ejecutada, Cancelada
        /// Ejemplo de request:
        ///
        ///     PATCH /api/orden/1/estado
        ///     "Ejecutada"
        ///
        /// </remarks>
        /// <param name="ordenId">Id de la orden a actualizar.</param>
        /// <param name="nuevoEstado">Nuevo estado de la orden.</param>
        /// <returns>Orden actualizada.</returns>
        [HttpPatch("{ordenId}/estado/enum")]
        [ProducesResponseType(typeof(OrdenDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize]
        public async Task<ActionResult<OrdenDto>> ActualizarEstadoOrden(int ordenId, [FromBody] EstadoDeOrden nuevoEstado)
        {
            var ordenActualizada = await _ordenService.ActualizarEstadoOrdenAsync(ordenId, nuevoEstado);
            return Ok(ordenActualizada);
        }

        /// PATCH: api/orden/{ordenId}/estado
        /// <summary>
        /// Actualiza el estado de una orden (RECIBE DESCRIPCION ESTADO).
        /// </summary>
        /// <remarks>
        /// Valores posibles para el estado: "EnProceso", "Ejecutada", "Cancelada"
        /// Ejemplo de request:
        ///
        ///     PATCH /api/orden/1/estado
        ///     "Ejecutada"
        ///
        /// </remarks>
        /// <param name="ordenId">Id de la orden a actualizar.</param>
        /// <param name="descripcionEstado">Nuevo estado de la orden.</param>
        /// <returns>Orden actualizada.</returns>
        [HttpPatch("{ordenId}/estado/descripcion")]
        [ProducesResponseType(typeof(OrdenDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize]
        public async Task<ActionResult<OrdenDto>> ActualizarEstadoOrdenByDescripicon(int ordenId, [FromBody] string descripcionEstado)
        {
            var ordenActualizada = await _ordenService.ActualizarEstadoOrdenAsync(ordenId, descripcionEstado);
            return Ok(ordenActualizada);
        }

        /// <summary>
        /// Elimina una orden existente.
        /// </summary>
        /// <param name="ordenId">Id de la orden a eliminar.</param>
        /// <returns>NoContent si se eliminó correctamente, NotFound si no existe.</returns>
        [HttpDelete("{ordenId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize]
        public async Task<ActionResult> EliminarOrden(int ordenId)
        {
            var eliminado = await _ordenService.EliminarOrdenAsync(ordenId);
            if (!eliminado)
                return NotFound();

            return NoContent();
        }

    }
}
