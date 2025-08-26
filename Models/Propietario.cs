using System.ComponentModel.DataAnnotations;

namespace Inmobiliaria.Models
{
    public class Propietario
    {
        public int Id { get; set; }

        [Required, StringLength(12)]
        public string DNI { get; set; }

        [Required, StringLength(50)]
        public string Apellido { get; set; }

        [Required, StringLength(50)]
        public string Nombre { get; set; }

        [Phone, Display(Name = "Teléfono")]
        public string Telefono { get; set; }

        [EmailAddress]
        public string Email { get; set; }
    }
}
