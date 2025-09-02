using Microsoft.Data.Sqlite;
using Inmobiliaria.Models;

namespace Inmobiliaria.Data
{
    public class RepositorioContrato
    {
        private readonly string cs;
        public RepositorioContrato(string connectionString) => cs = connectionString;

        public List<Contrato> ObtenerTodos()
        {
            var res = new List<Contrato>();
            using var conn = new SqliteConnection(cs);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                SELECT c.Id, c.IdInmueble, c.IdInquilino, c.FechaInicio, c.FechaFin, 
                       c.MontoMensual, c.Deposito, c.Estado,
                       i.Direccion as InmuebleDireccion,
                       iq.NombreCompleto as InquilinoNombre
                FROM Contratos c
                JOIN Inmuebles i ON i.Id = c.IdInmueble
                JOIN Inquilinos iq ON iq.Id = c.IdInquilino
                ORDER BY c.FechaInicio DESC;";
            using var r = cmd.ExecuteReader();
            while (r.Read())
            {
                res.Add(new Contrato
                {
                    Id = r.GetInt32(0),
                    IdInmueble = r.GetInt32(1),
                    IdInquilino = r.GetInt32(2),
                    FechaInicio = r.IsDBNull(3) ? null : r.GetString(3),
                    FechaFin = r.IsDBNull(4) ? null : r.GetString(4),
                    MontoMensual = r.GetDouble(5),
                    Deposito = r.IsDBNull(6) ? null : r.GetDouble(6),
                    Estado = r.IsDBNull(7) ? null : r.GetString(7),
                    InmuebleDireccion = r.IsDBNull(8) ? null : r.GetString(8),
                    InquilinoNombre = r.IsDBNull(9) ? null : r.GetString(9),
                });
            }
            return res;
        }

        public Contrato? ObtenerPorId(int id)
        {
            using var conn = new SqliteConnection(cs);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                SELECT c.Id, c.IdInmueble, c.IdInquilino, c.FechaInicio, c.FechaFin, 
                       c.MontoMensual, c.Deposito, c.Estado,
                       i.Direccion as InmuebleDireccion,
                       iq.NombreCompleto as InquilinoNombre
                FROM Contratos c
                JOIN Inmuebles i ON i.Id = c.IdInmueble
                JOIN Inquilinos iq ON iq.Id = c.IdInquilino
                WHERE c.Id=@Id;";
            cmd.Parameters.AddWithValue("@Id", id);
            using var r = cmd.ExecuteReader();
            if (!r.Read()) return null;
            return new Contrato
            {
                Id = r.GetInt32(0),
                IdInmueble = r.GetInt32(1),
                IdInquilino = r.GetInt32(2),
                FechaInicio = r.IsDBNull(3) ? null : r.GetString(3),
                FechaFin = r.IsDBNull(4) ? null : r.GetString(4),
                MontoMensual = r.GetDouble(5),
                Deposito = r.IsDBNull(6) ? null : r.GetDouble(6),
                Estado = r.IsDBNull(7) ? null : r.GetString(7),
                InmuebleDireccion = r.IsDBNull(8) ? null : r.GetString(8),
                InquilinoNombre = r.IsDBNull(9) ? null : r.GetString(9),
            };
        }

        public int Alta(Contrato x)
        {
            using var conn = new SqliteConnection(cs);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                INSERT INTO Contratos (IdInmueble, IdInquilino, FechaInicio, FechaFin, MontoMensual, Deposito, Estado)
                VALUES (@IdInmueble, @IdInquilino, @FechaInicio, @FechaFin, @MontoMensual, @Deposito, @Estado);
                SELECT last_insert_rowid();";
            cmd.Parameters.AddWithValue("@IdInmueble", x.IdInmueble);
            cmd.Parameters.AddWithValue("@IdInquilino", x.IdInquilino);
            cmd.Parameters.AddWithValue("@FechaInicio", x.FechaInicio ?? "");
            cmd.Parameters.AddWithValue("@FechaFin", x.FechaFin ?? "");
            cmd.Parameters.AddWithValue("@MontoMensual", x.MontoMensual);
            cmd.Parameters.AddWithValue("@Deposito", (object?)x.Deposito ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Estado", x.Estado ?? "Vigente");
            var id = Convert.ToInt32(cmd.ExecuteScalar());
            x.Id = id;
            return id;
        }

        public int Modificacion(Contrato x)
        {
            using var conn = new SqliteConnection(cs);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                UPDATE Contratos
                SET IdInmueble=@IdInmueble, IdInquilino=@IdInquilino, FechaInicio=@FechaInicio, FechaFin=@FechaFin,
                    MontoMensual=@MontoMensual, Deposito=@Deposito, Estado=@Estado
                WHERE Id=@Id;";
            cmd.Parameters.AddWithValue("@IdInmueble", x.IdInmueble);
            cmd.Parameters.AddWithValue("@IdInquilino", x.IdInquilino);
            cmd.Parameters.AddWithValue("@FechaInicio", x.FechaInicio ?? "");
            cmd.Parameters.AddWithValue("@FechaFin", x.FechaFin ?? "");
            cmd.Parameters.AddWithValue("@MontoMensual", x.MontoMensual);
            cmd.Parameters.AddWithValue("@Deposito", (object?)x.Deposito ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Estado", x.Estado ?? "Vigente");
            cmd.Parameters.AddWithValue("@Id", x.Id);
            return cmd.ExecuteNonQuery();
        }

        public int Baja(int id)
        {
            using var conn = new SqliteConnection(cs);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "DELETE FROM Contratos WHERE Id=@Id;";
            cmd.Parameters.AddWithValue("@Id", id);
            return cmd.ExecuteNonQuery();
        }
    }
}
