using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPiChallenge.Core.DTOs
{
    public class CuentaDto
    {
        public int IdCuenta { get; set; }
        public string Usuario { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public decimal Saldo { get; set; }
        public string Moneda { get; set; } = "ARS";
        public bool IsEnabled { get; set; }
    }
}
