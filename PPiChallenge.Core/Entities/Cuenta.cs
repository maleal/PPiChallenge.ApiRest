using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPiChallenge.Core.Entities
{
    public class Cuenta
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdCuenta { get; set; }

        [Required]
        [MaxLength(100)]
        public string Usuario { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(200)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        public decimal Saldo { get; set; } = 0;

        [Required]
        [MaxLength(3)]
        public string Moneda { get; set; } = "ARS";

        public bool IsEnabled { get; set; } = true;

        // Relación 1:N con Orden
        public ICollection<Orden> Ordenes { get; set; } = new List<Orden>();
    }
}
