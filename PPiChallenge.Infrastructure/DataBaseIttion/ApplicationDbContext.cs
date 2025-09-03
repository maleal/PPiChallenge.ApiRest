using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PPiChallenge.Core.Entities;

namespace PPiChallenge.Infrastructure.DataBaseIttion
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        //Las entidades (a crear) con migraciones
        public DbSet<Cuenta> Cuentas { get; set; }
        public DbSet<Orden> Orden { get; set; }
        public DbSet<ActivoFinanciero> activoFinancieros { get; set; }

        //Para inicializar ActivoFinanciero con los datos del Challenge
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //ActivoFinanciero
            //4 para que permita guardar hasta 4 decimales,
            //18 estándar para SQL Server y soporta valores grandes
            modelBuilder.Entity<ActivoFinanciero>()
                .Property(a => a.ComisionPorcentaje)
                .HasPrecision(18, 4);

            modelBuilder.Entity<ActivoFinanciero>()
                .Property(a => a.ImpuestoPorcentaje)
                .HasPrecision(18, 4);
            modelBuilder.Entity<ActivoFinanciero>()
                .Property(a => a.PrecioUnitario)
                .HasPrecision(18, 4);

            //Cuenta
            //tiene emails unicos
            modelBuilder.Entity<Cuenta>()
                .HasIndex(c => c.Email)
                .IsUnique();

            modelBuilder.Entity<Cuenta>()
                .Property(c => c.Saldo)
                .HasPrecision(18, 4);
            
            //Orden
            //Relación (1:N) Cuenta -> Orden 
            modelBuilder.Entity<Orden>()
                .HasOne<Cuenta>()
                .WithMany(c => c.Ordenes)
                .HasForeignKey(o => o.IdCuenta)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Orden>()
                .Property(o => o.Comision)
                .HasPrecision(18, 4);

            modelBuilder.Entity<Orden>()
                .Property(o => o.Impuesto)
                .HasPrecision(18, 4);

            modelBuilder.Entity<Orden>()
                .Property(o => o.MontoTotal)
                .HasPrecision(18, 4);

            modelBuilder.Entity<Orden>()
                .Property(o => o.Precio)
                .HasPrecision(18, 4);
            
            // Creamos un Usuario administrador
            modelBuilder.Entity<Cuenta>().HasData(
                new Cuenta
                {
                    IdCuenta = 1,
                    Usuario = "admin",
                    Email = "admin@ppi.com",
                    PasswordHash = "admin123", //ejemplo de un hash (debería ir un hash real)
                    Saldo = 100000,
                    Moneda = "ARS",
                    IsEnabled = true
                });

            // Cargamos dTipos de Activo
            modelBuilder.Entity<TipoActivo>().HasData(
                new TipoActivo { Id = 1, Descripcion = "Accion" },
                new TipoActivo { Id = 2, Descripcion = "Bono" },
                new TipoActivo { Id = 3, Descripcion = "FCI" }
            );

            // Cargamos Estados de Órdenes
            modelBuilder.Entity<EstadoOrden>().HasData(
                new EstadoOrden { Id = 1, DescripcionEstado = "En proceso" },
                new EstadoOrden { Id = 2, DescripcionEstado = "Ejecutada" },
                new EstadoOrden { Id = 3, DescripcionEstado = "Cancelada" }
            );

            // Cargamos los Activos Financieros
            modelBuilder.Entity<ActivoFinanciero>().HasData(
                new ActivoFinanciero { Id = 1, Ticker = "AAPL", Nombre = "Apple", TipoActivoId = 1, PrecioUnitario = 177.97M },
                new ActivoFinanciero { Id = 2, Ticker = "GOOGL", Nombre = "Alphabet Inc", TipoActivoId = 1, PrecioUnitario = 138.21M },
                new ActivoFinanciero { Id = 3, Ticker = "MSFT", Nombre = "Microsoft", TipoActivoId = 1, PrecioUnitario = 329.04M },
                new ActivoFinanciero { Id = 4, Ticker = "KO", Nombre = "Coca Cola", TipoActivoId = 1, PrecioUnitario = 58.30M },
                new ActivoFinanciero { Id = 5, Ticker = "WMT", Nombre = "Walmart", TipoActivoId = 1, PrecioUnitario = 163.42M },
                new ActivoFinanciero { Id = 6, Ticker = "AL30", Nombre = "BONOS ARGENTINA USD 2030 L.A", TipoActivoId = 2, PrecioUnitario = 307.40M },
                new ActivoFinanciero { Id = 7, Ticker = "GD30", Nombre = "Bonos Globales Argentina USD Step Up 2030", TipoActivoId = 2, PrecioUnitario = 336.10M },
                new ActivoFinanciero { Id = 8, Ticker = "Delta.Pesos", Nombre = "Delta Pesos Clase A", TipoActivoId = 3, PrecioUnitario = 0.0181M },
                new ActivoFinanciero { Id = 9, Ticker = "Fima.Premium", Nombre = "Fima Premium Clase A", TipoActivoId = 3, PrecioUnitario = 0.0317M }
            );

        }
    }
}
