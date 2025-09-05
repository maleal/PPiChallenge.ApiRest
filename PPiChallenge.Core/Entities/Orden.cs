using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPiChallenge.Core.Entities
{
    public enum EstadoDeOrden
    {
        EnProceso = 1,
        Ejecutada = 2,
        Cancelada = 3
    }
    public class Orden
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdOrden { get; set; }

        [Required]
        public int IdCuenta { get; set; }

        [Required]
        public int ActivoFinancieroId { get; set; }

        [ForeignKey(nameof(ActivoFinancieroId))]
        public ActivoFinanciero ActivoFinanciero { get; set; } = null!;

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Cantidad debe ser mayor que 0")]
        public int Cantidad { get; set; }

        [Required]
        [Precision(18, 4)]
        [Range(0.01, double.MaxValue, ErrorMessage = "Precio debe ser mayor que 0")]
        public decimal Precio { get; set; }

        [Required]
        [RegularExpression("^[CV]$", ErrorMessage = "Operaciones permitidas 'C'=Compra o 'V'=Venta")]
        public string Operacion { get; set; }

        //Descripcion Estado de la orden, inicializada a "EnProceso" segun requerimiento
        //NOTA: No agregare aqui a EstadoOrdenId ni la propiedad de navegacion porque el REquerimiento dice que es OPCIONAL el 'int Estado'
        [Required]
        public EstadoDeOrden Estado { get; set; } = EstadoDeOrden.EnProceso;

        // Campos calculados automáticamente
        [Precision(18, 4)]
        public decimal MontoTotal { get; set; } = 0;
        [Precision(18, 4)]
        public decimal Comision { get; set; } = 0;
        [Precision(18, 4)]
        public decimal Impuesto { get; set; } = 0;
    }
}
