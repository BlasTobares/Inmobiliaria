namespace Inmobiliaria.Models
{
    public class Contrato
    {
        public int Id { get; set; }
        public int IdInmueble { get; set; }
        public int IdInquilino { get; set; }
        public string? FechaInicio { get; set; } // yyyy-MM-dd
        public string? FechaFin { get; set; }
        public double MontoMensual { get; set; }
        public double? Deposito { get; set; }
        public string? Estado { get; set; } // Vigente/Finalizado/Rescindido

        // Para vistas
        public string? InmuebleDireccion { get; set; }
        public string? InquilinoNombre { get; set; }
    }
}
