using System.ComponentModel.DataAnnotations;

namespace Inmobiliaria.Models
{
    public class Inquilino
    {
        public int Id { get; set; }

        [Required, StringLength(12)]
        public string DNI { get; set; }

        [Required, StringLength(100), Display(Name = "Nombre completo")]
        public string NombreCompleto { get; set; }

        [Phone, Display(Name = "Teléfono")]
        public string Telefono { get; set; }

        [EmailAddress]
        public string Email { get; set; }
    }
}
