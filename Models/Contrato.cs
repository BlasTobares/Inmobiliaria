
using System.ComponentModel.DataAnnotations;

namespace Inmobiliaria.Models
{
    public class Contrato
    {
        public int Id { get; set; }
        public int IdInmueble { get; set; }
        public int IdInquilino { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Fecha de Inicio")]
        public DateTime FechaInicio { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Fecha de Fin")]
        public DateTime FechaFin { get; set; }

        [Display(Name = "Monto Mensual")]
        public double MontoMensual { get; set; }

        public double? Deposito { get; set; }
        public string? Estado { get; set; }

        // === Extras para mostrar ===
        public string? InmuebleDireccion { get; set; }
        public string? InquilinoNombre { get; set; }

        // === Auditoría (mapeo con campos de la tabla Contratos) ===
        public int? CreatedByUserId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public int? EndedByUserId { get; set; }
        public DateTime? EndedAt { get; set; }

        // Nombres legibles (JOIN con Usuarios)
        public string? CreatedByUserName { get; set; }
        public string? EndedByUserName { get; set; }
    }
}
