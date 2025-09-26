namespace Inmobiliaria.Models
{
    public class Pago
    {
        public int Id { get; set; }
        public int IdContrato { get; set; }
        public int NroPago { get; set; }
        public DateTime Fecha { get; set; }
        public double Importe { get; set; }
        public string? Detalle { get; set; }
        public string Estado { get; set; } = "Activo"; // Activo o Anulado

        // Auditoría
        public int? CreatedByUserId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public int? AnnulledByUserId { get; set; }
        public DateTime? AnnulledAt { get; set; }

        // Extras para mostrar
        public string? ContratoInfo { get; set; }
    }
}
