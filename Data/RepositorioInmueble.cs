using Microsoft.Data.Sqlite;
using Inmobiliaria.Models;

namespace Inmobiliaria.Data
{
    public class RepositorioInmueble
    {
        private readonly string cs;
        public RepositorioInmueble(string connectionString) => cs = connectionString;

        public List<Inmueble> ObtenerTodos()
        {
            var res = new List<Inmueble>();
            using var conn = new SqliteConnection(cs);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                SELECT i.Id, i.Direccion, i.Uso, i.Tipo, i.Ambientes, i.Superficie,
                       i.Latitud, i.Longitud, i.PrecioBase, i.IdPropietario, i.Estado,
                       (p.Apellido || ', ' || p.Nombre) as PropietarioNombre
                FROM Inmuebles i
                JOIN Propietarios p ON p.Id = i.IdPropietario
                ORDER BY i.Direccion;";
            using var r = cmd.ExecuteReader();
            while (r.Read())
            {
                // columna 10 = Estado (texto en la BD)
                var estadoStr = r.IsDBNull(10) ? "Disponible" : r.GetString(10);
                var estadoBool = estadoStr != null &&
                                 (estadoStr.Equals("Disponible", StringComparison.OrdinalIgnoreCase)
                                  || estadoStr == "1"
                                  || estadoStr.Equals("true", StringComparison.OrdinalIgnoreCase));

                res.Add(new Inmueble
                {
                    Id = r.GetInt32(0),
                    Direccion = r.IsDBNull(1) ? null : r.GetString(1),
                    Uso = r.IsDBNull(2) ? null : r.GetString(2),
                    Tipo = r.IsDBNull(3) ? null : r.GetString(3),
                    Ambientes = r.IsDBNull(4) ? 0 : r.GetInt32(4),
                    Superficie = r.IsDBNull(5) ? (double?)null : r.GetDouble(5),
                    Latitud = r.IsDBNull(6) ? (double?)null : r.GetDouble(6),
                    Longitud = r.IsDBNull(7) ? (double?)null : r.GetDouble(7),
                    PrecioBase = r.IsDBNull(8) ? 0.0 : r.GetDouble(8),
                    IdPropietario = r.IsDBNull(9) ? 0 : r.GetInt32(9),
                    Estado = estadoBool,
                    PropietarioNombre = r.IsDBNull(11) ? null : r.GetString(11),
                });
            }
            return res;
        }

        public List<Inmueble> ObtenerDisponibles()
        {
            var res = new List<Inmueble>();
            using var conn = new SqliteConnection(cs);
            conn.Open();
            using var cmd = conn.CreateCommand();
            // consideramos disponible si la columna es NULL o tiene el texto 'Disponible'
            cmd.CommandText = @"
                SELECT i.Id, i.Direccion, i.Uso, i.Tipo, i.Ambientes, i.Superficie,
                       i.Latitud, i.Longitud, i.PrecioBase, i.IdPropietario, i.Estado,
                       (p.Apellido || ', ' || p.Nombre) as PropietarioNombre
                FROM Inmuebles i
                JOIN Propietarios p ON p.Id = i.IdPropietario
                WHERE i.Estado IS NULL OR i.Estado = 'Disponible'
                ORDER BY i.Direccion;";
            using var r = cmd.ExecuteReader();
            while (r.Read())
            {
                var estadoStr = r.IsDBNull(10) ? "Disponible" : r.GetString(10);
                var estadoBool = estadoStr != null &&
                                 (estadoStr.Equals("Disponible", StringComparison.OrdinalIgnoreCase)
                                  || estadoStr == "1"
                                  || estadoStr.Equals("true", StringComparison.OrdinalIgnoreCase));

                res.Add(new Inmueble
                {
                    Id = r.GetInt32(0),
                    Direccion = r.IsDBNull(1) ? null : r.GetString(1),
                    Uso = r.IsDBNull(2) ? null : r.GetString(2),
                    Tipo = r.IsDBNull(3) ? null : r.GetString(3),
                    Ambientes = r.IsDBNull(4) ? 0 : r.GetInt32(4),
                    Superficie = r.IsDBNull(5) ? (double?)null : r.GetDouble(5),
                    Latitud = r.IsDBNull(6) ? (double?)null : r.GetDouble(6),
                    Longitud = r.IsDBNull(7) ? (double?)null : r.GetDouble(7),
                    PrecioBase = r.IsDBNull(8) ? 0.0 : r.GetDouble(8),
                    IdPropietario = r.IsDBNull(9) ? 0 : r.GetInt32(9),
                    Estado = estadoBool,
                    PropietarioNombre = r.IsDBNull(11) ? null : r.GetString(11),
                });
            }
            return res;
        }

        public Inmueble? ObtenerPorId(int id)
        {
            using var conn = new SqliteConnection(cs);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                SELECT i.Id, i.Direccion, i.Uso, i.Tipo, i.Ambientes, i.Superficie,
                       i.Latitud, i.Longitud, i.PrecioBase, i.IdPropietario, i.Estado,
                       (p.Apellido || ', ' || p.Nombre) as PropietarioNombre
                FROM Inmuebles i
                JOIN Propietarios p ON p.Id = i.IdPropietario
                WHERE i.Id=@Id;";
            cmd.Parameters.AddWithValue("@Id", id);
            using var r = cmd.ExecuteReader();
            if (!r.Read()) return null;

            var estadoStr = r.IsDBNull(10) ? "Disponible" : r.GetString(10);
            var estadoBool = estadoStr != null &&
                             (estadoStr.Equals("Disponible", StringComparison.OrdinalIgnoreCase)
                              || estadoStr == "1"
                              || estadoStr.Equals("true", StringComparison.OrdinalIgnoreCase));

            return new Inmueble
            {
                Id = r.GetInt32(0),
                Direccion = r.IsDBNull(1) ? null : r.GetString(1),
                Uso = r.IsDBNull(2) ? null : r.GetString(2),
                Tipo = r.IsDBNull(3) ? null : r.GetString(3),
                Ambientes = r.IsDBNull(4) ? 0 : r.GetInt32(4),
                Superficie = r.IsDBNull(5) ? (double?)null : r.GetDouble(5),
                Latitud = r.IsDBNull(6) ? (double?)null : r.GetDouble(6),
                Longitud = r.IsDBNull(7) ? (double?)null : r.GetDouble(7),
                PrecioBase = r.IsDBNull(8) ? 0.0 : r.GetDouble(8),
                IdPropietario = r.IsDBNull(9) ? 0 : r.GetInt32(9),
                Estado = estadoBool,
                PropietarioNombre = r.IsDBNull(11) ? null : r.GetString(11),
            };
        }

        public int Alta(Inmueble x)
        {
            using var conn = new SqliteConnection(cs);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                INSERT INTO Inmuebles 
                (Direccion, Uso, Tipo, Ambientes, Superficie, Latitud, Longitud, PrecioBase, IdPropietario, Estado)
                VALUES (@Direccion, @Uso, @Tipo, @Ambientes, @Superficie, @Latitud, @Longitud, @PrecioBase, @IdPropietario, @Estado);
                SELECT last_insert_rowid();";
            cmd.Parameters.AddWithValue("@Direccion", x.Direccion ?? "");
            cmd.Parameters.AddWithValue("@Uso", x.Uso ?? "");
            cmd.Parameters.AddWithValue("@Tipo", x.Tipo ?? "");
            cmd.Parameters.AddWithValue("@Ambientes", x.Ambientes);
            cmd.Parameters.AddWithValue("@Superficie", (object?)x.Superficie ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Latitud", (object?)x.Latitud ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Longitud", (object?)x.Longitud ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@PrecioBase", x.PrecioBase);
            cmd.Parameters.AddWithValue("@IdPropietario", x.IdPropietario);
            // convertimos bool a texto para la BD
            cmd.Parameters.AddWithValue("@Estado", x.Estado ? "Disponible" : "No disponible");
            var id = Convert.ToInt32(cmd.ExecuteScalar());
            x.Id = id;
            return id;
        }

        public int Modificacion(Inmueble x)
        {
            using var conn = new SqliteConnection(cs);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                UPDATE Inmuebles
                SET Direccion=@Direccion, Uso=@Uso, Tipo=@Tipo, Ambientes=@Ambientes, Superficie=@Superficie,
                    Latitud=@Latitud, Longitud=@Longitud, PrecioBase=@PrecioBase, IdPropietario=@IdPropietario, Estado=@Estado
                WHERE Id=@Id;";
            cmd.Parameters.AddWithValue("@Direccion", x.Direccion ?? "");
            cmd.Parameters.AddWithValue("@Uso", x.Uso ?? "");
            cmd.Parameters.AddWithValue("@Tipo", x.Tipo ?? "");
            cmd.Parameters.AddWithValue("@Ambientes", x.Ambientes);
            cmd.Parameters.AddWithValue("@Superficie", (object?)x.Superficie ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Latitud", (object?)x.Latitud ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Longitud", (object?)x.Longitud ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@PrecioBase", x.PrecioBase);
            cmd.Parameters.AddWithValue("@IdPropietario", x.IdPropietario);
            cmd.Parameters.AddWithValue("@Estado", x.Estado ? "Disponible" : "No disponible");
            cmd.Parameters.AddWithValue("@Id", x.Id);
            return cmd.ExecuteNonQuery();
        }

        /*public int Baja(int id)
        {
            using var conn = new SqliteConnection(cs);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "DELETE FROM Inmuebles WHERE Id=@Id;";
            cmd.Parameters.AddWithValue("@Id", id);
            return cmd.ExecuteNonQuery();
        }
    }
}
*/

public List<Inmueble> ObtenerNoOcupadosEntre(DateTime fechaInicio, DateTime fechaFin)
{
    var res = new List<Inmueble>();
    using var conn = new SqliteConnection(cs);
    conn.Open();
    using var cmd = conn.CreateCommand();

    cmd.CommandText = @"
        SELECT i.Id, i.Direccion, i.Uso, i.Tipo, i.Ambientes, i.Superficie,
               i.Latitud, i.Longitud, i.PrecioBase, i.IdPropietario, i.Estado,
               (p.Apellido || ', ' || p.Nombre) as PropietarioNombre
        FROM Inmuebles i
        JOIN Propietarios p ON p.Id = i.IdPropietario
        WHERE i.Estado = 'Disponible'
          AND i.Id NOT IN (
              SELECT c.IdInmueble
              FROM Contratos c
              WHERE (c.FechaInicio <= @FechaFin AND c.FechaFin >= @FechaInicio)
          )
        ORDER BY i.Direccion;
    ";

    // uso formato ISO para que la comparación textual de fechas funcione correctamente en SQLite
    cmd.Parameters.AddWithValue("@FechaInicio", fechaInicio.ToString("yyyy-MM-dd"));
    cmd.Parameters.AddWithValue("@FechaFin", fechaFin.ToString("yyyy-MM-dd"));

    using var r = cmd.ExecuteReader();
    while (r.Read())
    {
        res.Add(new Inmueble
        {
            Id = r.GetInt32(0),
            Direccion = r.IsDBNull(1) ? null : r.GetString(1),
            Uso = r.IsDBNull(2) ? null : r.GetString(2),
            Tipo = r.IsDBNull(3) ? null : r.GetString(3),
            Ambientes = r.IsDBNull(4) ? 0 : r.GetInt32(4),
            Superficie = r.IsDBNull(5) ? (double?)null : r.GetDouble(5),
            Latitud = r.IsDBNull(6) ? (double?)null : r.GetDouble(6),
            Longitud = r.IsDBNull(7) ? (double?)null : r.GetDouble(7),
            PrecioBase = r.IsDBNull(8) ? 0.0 : r.GetDouble(8),
            IdPropietario = r.IsDBNull(9) ? 0 : r.GetInt32(9),
            Estado = !r.IsDBNull(10) && r.GetBoolean(10),
            PropietarioNombre = r.IsDBNull(11) ? null : r.GetString(11)
        });
    }

    return res;
}


        public int Baja(int id)
        {
            try
            {
                using var conn = new SqliteConnection(cs);
                conn.Open();
                using var cmd = conn.CreateCommand();
                cmd.CommandText = "DELETE FROM Inmuebles WHERE Id=@Id;";
                cmd.Parameters.AddWithValue("@Id", id);
                return cmd.ExecuteNonQuery();
            }
            catch (Microsoft.Data.Sqlite.SqliteException ex)
            {
                // Código 19 = constraint failed (foreign key, unique, etc.)
                if (ex.SqliteErrorCode == 19)
                {
                    throw new InvalidOperationException("No se puede eliminar el inmueble porque tiene contratos u otros registros asociados.");
                }
                throw;
            }
        }
    }
}


