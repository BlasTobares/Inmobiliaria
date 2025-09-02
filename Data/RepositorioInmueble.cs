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
                SELECT i.Id, i.Direccion, i.Uso, i.Tipo, i.Ambientes, i.Superficie, i.Latitud, i.Longitud, 
                       i.PrecioBase, i.IdPropietario,
                       (p.Apellido || ', ' || p.Nombre) as PropietarioNombre
                FROM Inmuebles i
                JOIN Propietarios p ON p.Id = i.IdPropietario
                ORDER BY i.Direccion;";
            using var r = cmd.ExecuteReader();
            while (r.Read())
            {
                res.Add(new Inmueble
                {
                    Id = r.GetInt32(0),
                    Direccion = r.IsDBNull(1) ? null : r.GetString(1),
                    Uso = r.IsDBNull(2) ? null : r.GetString(2),
                    Tipo = r.IsDBNull(3) ? null : r.GetString(3),
                    Ambientes = r.GetInt32(4),
                    Superficie = r.IsDBNull(5) ? null : r.GetDouble(5),
                    Latitud = r.IsDBNull(6) ? null : r.GetDouble(6),
                    Longitud = r.IsDBNull(7) ? null : r.GetDouble(7),
                    PrecioBase = r.GetDouble(8),
                    IdPropietario = r.GetInt32(9),
                    PropietarioNombre = r.IsDBNull(10) ? null : r.GetString(10),
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
                SELECT i.Id, i.Direccion, i.Uso, i.Tipo, i.Ambientes, i.Superficie, i.Latitud, i.Longitud, 
                       i.PrecioBase, i.IdPropietario,
                       (p.Apellido || ', ' || p.Nombre) as PropietarioNombre
                FROM Inmuebles i
                JOIN Propietarios p ON p.Id = i.IdPropietario
                WHERE i.Id=@Id;";
            cmd.Parameters.AddWithValue("@Id", id);
            using var r = cmd.ExecuteReader();
            if (!r.Read()) return null;
            return new Inmueble
            {
                Id = r.GetInt32(0),
                Direccion = r.IsDBNull(1) ? null : r.GetString(1),
                Uso = r.IsDBNull(2) ? null : r.GetString(2),
                Tipo = r.IsDBNull(3) ? null : r.GetString(3),
                Ambientes = r.GetInt32(4),
                Superficie = r.IsDBNull(5) ? null : r.GetDouble(5),
                Latitud = r.IsDBNull(6) ? null : r.GetDouble(6),
                Longitud = r.IsDBNull(7) ? null : r.GetDouble(7),
                PrecioBase = r.GetDouble(8),
                IdPropietario = r.GetInt32(9),
                PropietarioNombre = r.IsDBNull(10) ? null : r.GetString(10),
            };
        }

        public int Alta(Inmueble x)
        {
            using var conn = new SqliteConnection(cs);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                INSERT INTO Inmuebles (Direccion, Uso, Tipo, Ambientes, Superficie, Latitud, Longitud, PrecioBase, IdPropietario)
                VALUES (@Direccion, @Uso, @Tipo, @Ambientes, @Superficie, @Latitud, @Longitud, @PrecioBase, @IdPropietario);
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
                    Latitud=@Latitud, Longitud=@Longitud, PrecioBase=@PrecioBase, IdPropietario=@IdPropietario
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
            cmd.Parameters.AddWithValue("@Id", x.Id);
            return cmd.ExecuteNonQuery();
        }

        public int Baja(int id)
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
