using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PPiChallenge.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class PPiChallenge : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cuentas",
                columns: table => new
                {
                    IdCuenta = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Usuario = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Saldo = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    Moneda = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    IsEnabled = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cuentas", x => x.IdCuenta);
                });

            migrationBuilder.CreateTable(
                name: "EstadoOrden",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DescripcionEstado = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstadoOrden", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TipoActivo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TipoActivo", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "activoFinancieros",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ticker = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    TipoActivoId = table.Column<int>(type: "int", nullable: false),
                    PrecioUnitario = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    PrecioSeRecibe = table.Column<bool>(type: "bit", nullable: false),
                    ComisionPorcentaje = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    ImpuestoPorcentaje = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_activoFinancieros", x => x.Id);
                    table.ForeignKey(
                        name: "FK_activoFinancieros_TipoActivo_TipoActivoId",
                        column: x => x.TipoActivoId,
                        principalTable: "TipoActivo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Orden",
                columns: table => new
                {
                    IdOrden = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdCuenta = table.Column<int>(type: "int", nullable: false),
                    ActivoFinancieroId = table.Column<int>(type: "int", nullable: false),
                    Cantidad = table.Column<int>(type: "int", nullable: false),
                    Precio = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    Operacion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    MontoTotal = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    Comision = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    Impuesto = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orden", x => x.IdOrden);
                    table.ForeignKey(
                        name: "FK_Orden_Cuentas_IdCuenta",
                        column: x => x.IdCuenta,
                        principalTable: "Cuentas",
                        principalColumn: "IdCuenta",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Orden_activoFinancieros_ActivoFinancieroId",
                        column: x => x.ActivoFinancieroId,
                        principalTable: "activoFinancieros",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Cuentas",
                columns: new[] { "IdCuenta", "Email", "IsEnabled", "Moneda", "PasswordHash", "Saldo", "Usuario" },
                values: new object[] { 1, "admin@ppi.com", true, "ARS", "admin123", 100000m, "admin" });

            migrationBuilder.InsertData(
                table: "EstadoOrden",
                columns: new[] { "Id", "DescripcionEstado" },
                values: new object[,]
                {
                    { 1, "En proceso" },
                    { 2, "Ejecutada" },
                    { 3, "Cancelada" }
                });

            migrationBuilder.InsertData(
                table: "TipoActivo",
                columns: new[] { "Id", "Descripcion" },
                values: new object[,]
                {
                    { 1, "Accion" },
                    { 2, "Bono" },
                    { 3, "FCI" }
                });

            migrationBuilder.InsertData(
                table: "activoFinancieros",
                columns: new[] { "Id", "ComisionPorcentaje", "ImpuestoPorcentaje", "Nombre", "PrecioSeRecibe", "PrecioUnitario", "Ticker", "TipoActivoId" },
                values: new object[,]
                {
                    { 1, 0m, null, "Apple", true, 177.97m, "AAPL", 1 },
                    { 2, 0m, null, "Alphabet Inc", true, 138.21m, "GOOGL", 1 },
                    { 3, 0m, null, "Microsoft", true, 329.04m, "MSFT", 1 },
                    { 4, 0m, null, "Coca Cola", true, 58.30m, "KO", 1 },
                    { 5, 0m, null, "Walmart", true, 163.42m, "WMT", 1 },
                    { 6, 0m, null, "BONOS ARGENTINA USD 2030 L.A", true, 307.40m, "AL30", 2 },
                    { 7, 0m, null, "Bonos Globales Argentina USD Step Up 2030", true, 336.10m, "GD30", 2 },
                    { 8, 0m, null, "Delta Pesos Clase A", true, 0.0181m, "Delta.Pesos", 3 },
                    { 9, 0m, null, "Fima Premium Clase A", true, 0.0317m, "Fima.Premium", 3 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_activoFinancieros_TipoActivoId",
                table: "activoFinancieros",
                column: "TipoActivoId");

            migrationBuilder.CreateIndex(
                name: "IX_Cuentas_Email",
                table: "Cuentas",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orden_ActivoFinancieroId",
                table: "Orden",
                column: "ActivoFinancieroId");

            migrationBuilder.CreateIndex(
                name: "IX_Orden_IdCuenta",
                table: "Orden",
                column: "IdCuenta");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EstadoOrden");

            migrationBuilder.DropTable(
                name: "Orden");

            migrationBuilder.DropTable(
                name: "Cuentas");

            migrationBuilder.DropTable(
                name: "activoFinancieros");

            migrationBuilder.DropTable(
                name: "TipoActivo");
        }
    }
}
