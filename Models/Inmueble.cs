namespace Inmobiliaria.Models
{
    public class Inmueble
    {
        public int Id { get; set; }
        public string? Direccion { get; set; }
        public string? Uso { get; set; }       // Residencial / Comercial
        public string? Tipo { get; set; }      // Casa / Depto / Local
        public int Ambientes { get; set; }
        public double? Superficie { get; set; }
        public double? Latitud { get; set; }
        public double? Longitud { get; set; }
        public double PrecioBase { get; set; }
        public int IdPropietario { get; set; }

        // Para vistas (mostrar nombres sin otra consulta)
        public string? PropietarioNombre { get; set; }
    }
}
