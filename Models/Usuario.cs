namespace Inmobiliaria.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Email { get; set; } = "";
        public string Nombre { get; set; } = "";
        public string? Apellido { get; set; }
        public string PasswordHash { get; set; } = "";
        public string Rol { get; set; } = "Empleado"; // por defecto
        public string? AvatarPath { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
