using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPiChallenge.Core.Entities
{
    //public enum TipoActivo
    //{
    //    Accion = 1,
    //    Bono = 2,
    //    FCI = 3
    //}
    public class ActivoFinanciero
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(16)]
        public string Ticker { get; set; } = string.Empty;

        [Required]
        [MaxLength(64)]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        public int TipoActivoId { get; set; }

        [Required]
        public TipoActivo TipoActivo { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal PrecioUnitario { get; set; }

        public bool PrecioSeRecibe { get; set; } = true;
        
        [Precision(5, 4)]
        public decimal? ComisionPorcentaje { get; set; } = 0;

        [Precision(5, 4)]
        public decimal? ImpuestoPorcentaje { get; set; }
        public ICollection<Orden> Ordenes { get; set; } = new List<Orden>();
    }
}
