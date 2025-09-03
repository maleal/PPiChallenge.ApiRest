using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPiChallenge.Core.DTOs
{
    //DTO para la respuesta
    public class OrdenDto
    {
        public int Id { get; set; }
        public int CuentaId { get; set; }
        public int ActivoFinancieroId { get; set; }
        public int Cantidad { get; set; }
        public string Operacion { get; set; }
        public string Estado { get; set; }
        
        //Valores OUTPUT que calcularemos con la API
        public decimal Precio { get; set; }
        public decimal Comision { get; set; }
        public decimal Impuesto { get; set; }
        public decimal MontoTotal { get; set; }
    }
}
