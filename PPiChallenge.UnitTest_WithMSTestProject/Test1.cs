using Microsoft.Extensions.Logging;
using Moq;
using PPiChallenge.Core.DTOs;
using PPiChallenge.Core.Entities;
using PPiChallenge.Core.Interfaces;
using PPiChallenge.Infrastructure.Interfaces.Services;

namespace PPiChallenge.UnitTest_WithMSTestProject
{
    [TestClass]
    public class OrdenServiceTests
    {
        private Mock<IUnitOfWork> _mockUnitOfWork;
        private Mock<ILogger<OrdenService>> _mockLogger;
        private OrdenService _ordenService;

        [TestInitialize]
        public void Setup()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockLogger = new Mock<ILogger<OrdenService>>();
            _ordenService = new OrdenService(_mockUnitOfWork.Object, _mockLogger.Object);
        }

        [TestMethod]
        public async Task CrearOrdenAsync_Para_Accion()
        {
            //Armo entrada = CrearOrdenDto (Request)
            var dto = new CrearOrdenDto
            {
                CuentaId = 1,
                //ActivoFinancieroId = 1,
                Ticker = "AAPL",
                Cantidad = 100,
                Operacion = "C"
            };

            var cuenta = new Cuenta { IdCuenta = 1, IsEnabled = true };
            var tipoAccion = new TipoActivo { Id = 1, Descripcion = "Accion" };

            var activo = new ActivoFinanciero
            {
                Ticker = "AAPL",
                TipoActivoId = tipoAccion.Id,
                TipoActivo = tipoAccion,
                PrecioUnitario = 177.9700M, //esta en la BD !!
                
            };
            //Fin Armo entrada


            _mockUnitOfWork.Setup(u => u.Cuentas.GetByIdAsync(dto.CuentaId)).ReturnsAsync(cuenta);
            _mockUnitOfWork.Setup(u => u.ActivosFinancieros.GetByTickerWithTipoActivoAsync(dto.Ticker)).ReturnsAsync(activo);
            _mockUnitOfWork.Setup(u => u.Ordenes.AddAsync(It.IsAny<Orden>())).ReturnsAsync(true);
            _mockUnitOfWork.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            //Creo Orden
            var resultado = await _ordenService.CrearOrdenAsync(dto);

            //Valores que espero se cumplan (segun doc challenge)
            var montoEsperado = 177.9700M * 100;
            var comisionEsperada = montoEsperado * 0.006m; //
            var impuestoEsperado = comisionEsperada * 0.21m; //
            var totalEsperado = montoEsperado + comisionEsperada + impuestoEsperado; //17926.206220000M

            Assert.AreEqual(totalEsperado, resultado.MontoTotal, 0.001m, "Revisar Monto total que esta mal calculado");
            Assert.AreEqual(comisionEsperada, resultado.Comision, 0.001m, "Revisar Comisión que esta mal calculada");
            Assert.AreEqual(impuestoEsperado, resultado.Impuesto, 0.001m, "Revisar Impuesto que esta mal calculado");
        }


        [TestMethod]
        public async Task CrearOrdenAsync_Para_Bono()
        {
            //Armo entrada = CrearOrdenDto (Request)
            var dto = new CrearOrdenDto
            {
                CuentaId = 1,
                Ticker = "AL30", //ActivoFinancieroId = 2 es un Bono
                Cantidad = 10,
                PrecioUnitario = 100m,
                Operacion = "C"
            };

            var cuenta = new Cuenta { IdCuenta = dto.CuentaId, IsEnabled = true };
            var activo = new ActivoFinanciero
            {
                Ticker = "AL30",
                TipoActivo = new TipoActivo { Descripcion = "Bono" },
                PrecioUnitario = dto.PrecioUnitario ?? 0m
            };
            //Fin Armo entrada

            _mockUnitOfWork.Setup(u => u.Cuentas.GetByIdAsync(dto.CuentaId)).ReturnsAsync(cuenta);
            _mockUnitOfWork.Setup(u => u.ActivosFinancieros.GetByTickerWithTipoActivoAsync(dto.Ticker)).ReturnsAsync(activo);
            _mockUnitOfWork.Setup(u => u.Ordenes.AddAsync(It.IsAny<Orden>())).ReturnsAsync(true);
            _mockUnitOfWork.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            //Valores que espero se cumplan (segun doc challenge)
            decimal montoBase = dto.PrecioUnitario.Value * dto.Cantidad;
            decimal comisionEsperada = decimal.Round(montoBase * 0.002m, 4);
            decimal impuestoEsperado = decimal.Round(comisionEsperada * 0.21m, 4);
            decimal montoTotalEsperado = decimal.Round(montoBase + comisionEsperada + impuestoEsperado, 4);

            //Creo Orden
            var orden = await _ordenService.CrearOrdenAsync(dto);

            //Assert
            Assert.AreEqual(montoTotalEsperado, decimal.Round(orden.MontoTotal, 4));
            Assert.AreEqual(comisionEsperada, decimal.Round(orden.Comision, 4));
            Assert.AreEqual(impuestoEsperado, decimal.Round(orden.Impuesto, 4));
        }


        [TestMethod]
        public async Task CrearOrdenAsync_Para_FCI()
        {
            //Armo entrada = CrearOrdenDto (Request)
            var dto = new CrearOrdenDto
            {
                CuentaId = 1,
                Ticker = "FCI01",
                Cantidad = 10,
                Operacion = "C",
                PrecioUnitario = 50m
            };

            var cuenta = new Cuenta { IdCuenta = dto.CuentaId, IsEnabled = true };
            var activo = new ActivoFinanciero
            {
                Ticker = "FCI01",
                TipoActivo = new TipoActivo { Id = 3, Descripcion = "FCI" },
                PrecioUnitario = dto.PrecioUnitario ?? 0m
            };

            _mockUnitOfWork.Setup(u => u.Cuentas.GetByIdAsync(dto.CuentaId)).ReturnsAsync(cuenta);
            _mockUnitOfWork.Setup(u => u.ActivosFinancieros.GetByTickerWithTipoActivoAsync(dto.Ticker)).ReturnsAsync(activo);
            _mockUnitOfWork.Setup(u => u.Ordenes.AddAsync(It.IsAny<Orden>())).ReturnsAsync(true);
            _mockUnitOfWork.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            //
            var orden = await _ordenService.CrearOrdenAsync(dto);

            //Assert
            decimal totalEsperado = decimal.Round((decimal)dto.Cantidad * (dto.PrecioUnitario ?? 0m), 4);
            Assert.AreEqual(totalEsperado, decimal.Round(orden.MontoTotal, 4));
        }













    }
}
