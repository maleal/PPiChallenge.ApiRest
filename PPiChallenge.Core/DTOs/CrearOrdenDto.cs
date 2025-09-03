using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPiChallenge.Core.DTOs
{
    //DTOs para el requests
    public class CrearOrdenDto
    {
        [Required(ErrorMessage = "El Id de la cuenta es obligatorio.")]
        public int CuentaId { get; set; }
        [Required(ErrorMessage = "El Ticker/Codigo del activo es obligatorio.")]
        public string Ticker { get; set; } = string.Empty;
        public decimal? PrecioUnitario { get; set; } //Obligatorio para FCI o Bono
        [Required(ErrorMessage = "La cantidad es obligatoria.")]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0.")]
        public int Cantidad { get; set; }
        [Required(ErrorMessage = "La operación es obligatoria.")]
        [RegularExpression("C|V", ErrorMessage = "La operación debe ser 'C' (Compra) o 'V' (Venta).")]
        public string Operacion { get; set; } //"C" o "V"
    }
}
