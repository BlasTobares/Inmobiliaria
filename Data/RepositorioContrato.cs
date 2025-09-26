using Microsoft.Data.Sqlite;
using Inmobiliaria.Models;

namespace Inmobiliaria.Data
{
    public class RepositorioContrato
    {
        private readonly string cs;
        public RepositorioContrato(string connectionString) => cs = connectionString;

        // === OBTENER TODOS ===
        public List<Contrato> ObtenerTodos()
        {
            var lista = new List<Contrato>();
            using var conn = new SqliteConnection(cs);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                SELECT c.Id, c.IdInmueble, c.IdInquilino,
                       c.FechaInicio, c.FechaFin,
                       c.MontoMensual, c.Deposito, c.Estado,
                       c.CreatedByUserId, c.CreatedAt, c.EndedByUserId, c.EndedAt,
                       cu.Nombre || ' ' || cu.Apellido as CreatedByName,
                       eu.Nombre || ' ' || eu.Apellido as EndedByName,
                       i.Direccion AS InmuebleDireccion,
                       iq.NombreCompleto AS InquilinoNombre
                FROM Contratos c
                JOIN Inmuebles i ON i.Id = c.IdInmueble
                JOIN Inquilinos iq ON iq.Id = c.IdInquilino
                LEFT JOIN Usuarios cu ON cu.Id = c.CreatedByUserId
                LEFT JOIN Usuarios eu ON eu.Id = c.EndedByUserId
                ORDER BY c.FechaInicio DESC;";
            using var r = cmd.ExecuteReader();
            while (r.Read())
            {
                lista.Add(new Contrato
                {
                    Id = r.GetInt32(0),
                    IdInmueble = r.GetInt32(1),
                    IdInquilino = r.GetInt32(2),
                    FechaInicio = DateTime.Parse(r.GetString(3)),
                    FechaFin = DateTime.Parse(r.GetString(4)),
                    MontoMensual = r.GetDouble(5),
                    Deposito = r.IsDBNull(6) ? null : r.GetDouble(6),
                    Estado = r.IsDBNull(7) ? null : r.GetString(7),

                    CreatedByUserId = r.IsDBNull(8) ? null : r.GetInt32(8),
                    CreatedAt = r.IsDBNull(9) ? null : DateTime.Parse(r.GetString(9)),
                    EndedByUserId = r.IsDBNull(10) ? null : r.GetInt32(10),
                    EndedAt = r.IsDBNull(11) ? null : (DateTime?)DateTime.Parse(r.GetString(11)),
                    CreatedByUserName = r.IsDBNull(12) ? null : r.GetString(12),
                    EndedByUserName = r.IsDBNull(13) ? null : r.GetString(13),

                    InmuebleDireccion = r.IsDBNull(14) ? null : r.GetString(14),
                    InquilinoNombre = r.IsDBNull(15) ? null : r.GetString(15),
                });
            }
            return lista;
        }

        // === OBTENER POR ID ===
        public Contrato? ObtenerPorId(int id)
        {
            using var conn = new SqliteConnection(cs);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                SELECT c.Id, c.IdInmueble, c.IdInquilino,
                       c.FechaInicio, c.FechaFin,
                       c.MontoMensual, c.Deposito, c.Estado,
                       c.CreatedByUserId, c.CreatedAt, c.EndedByUserId, c.EndedAt,
                       cu.Nombre || ' ' || cu.Apellido as CreatedByName,
                       eu.Nombre || ' ' || eu.Apellido as EndedByName,
                       i.Direccion AS InmuebleDireccion,
                       iq.NombreCompleto AS InquilinoNombre
                FROM Contratos c
                JOIN Inmuebles i ON i.Id = c.IdInmueble
                JOIN Inquilinos iq ON iq.Id = c.IdInquilino
                LEFT JOIN Usuarios cu ON cu.Id = c.CreatedByUserId
                LEFT JOIN Usuarios eu ON eu.Id = c.EndedByUserId
                WHERE c.Id=@Id;";
            cmd.Parameters.AddWithValue("@Id", id);
            using var r = cmd.ExecuteReader();
            if (!r.Read()) return null;
            return new Contrato
            {
                Id = r.GetInt32(0),
                IdInmueble = r.GetInt32(1),
                IdInquilino = r.GetInt32(2),
                FechaInicio = DateTime.Parse(r.GetString(3)),
                FechaFin = DateTime.Parse(r.GetString(4)),
                MontoMensual = r.GetDouble(5),
                Deposito = r.IsDBNull(6) ? null : r.GetDouble(6),
                Estado = r.IsDBNull(7) ? null : r.GetString(7),

                CreatedByUserId = r.IsDBNull(8) ? null : r.GetInt32(8),
                CreatedAt = r.IsDBNull(9) ? null : DateTime.Parse(r.GetString(9)),
                EndedByUserId = r.IsDBNull(10) ? null : r.GetInt32(10),
                EndedAt = r.IsDBNull(11) ? null : (DateTime?)DateTime.Parse(r.GetString(11)),
                CreatedByUserName = r.IsDBNull(12) ? null : r.GetString(12),
                EndedByUserName = r.IsDBNull(13) ? null : r.GetString(13),

                InmuebleDireccion = r.IsDBNull(14) ? null : r.GetString(14),
                InquilinoNombre = r.IsDBNull(15) ? null : r.GetString(15),
            };
        }

        // === ALTA con auditoría opcional ===
        public int Alta(Contrato x, int? userId = null)
        {
            using var conn = new SqliteConnection(cs);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                INSERT INTO Contratos 
                (IdInmueble, IdInquilino, FechaInicio, FechaFin, MontoMensual, Deposito, Estado, CreatedByUserId, CreatedAt)
                VALUES (@IdInmueble, @IdInquilino, @FechaInicio, @FechaFin, @MontoMensual, @Deposito, @Estado, @UserId, datetime('now'));
                SELECT last_insert_rowid();";
            cmd.Parameters.AddWithValue("@IdInmueble", x.IdInmueble);
            cmd.Parameters.AddWithValue("@IdInquilino", x.IdInquilino);
            cmd.Parameters.AddWithValue("@FechaInicio", x.FechaInicio.ToString("yyyy-MM-dd"));
            cmd.Parameters.AddWithValue("@FechaFin", x.FechaFin.ToString("yyyy-MM-dd"));
            cmd.Parameters.AddWithValue("@MontoMensual", x.MontoMensual);
            cmd.Parameters.AddWithValue("@Deposito", (object?)x.Deposito ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Estado", x.Estado ?? "Vigente");
            cmd.Parameters.AddWithValue("@UserId", userId.HasValue ? (object)userId.Value : DBNull.Value);

            var id = Convert.ToInt32(cmd.ExecuteScalar());
            x.Id = id;
            return id;
        }

        // === MODIFICAR ===
        public int Modificacion(Contrato x)
        {
            using var conn = new SqliteConnection(cs);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                UPDATE Contratos
                SET IdInmueble=@IdInmueble, IdInquilino=@IdInquilino,
                    FechaInicio=@FechaInicio, FechaFin=@FechaFin,
                    MontoMensual=@MontoMensual, Deposito=@Deposito, Estado=@Estado
                WHERE Id=@Id;";
            cmd.Parameters.AddWithValue("@IdInmueble", x.IdInmueble);
            cmd.Parameters.AddWithValue("@IdInquilino", x.IdInquilino);
            cmd.Parameters.AddWithValue("@FechaInicio", x.FechaInicio.ToString("yyyy-MM-dd"));
            cmd.Parameters.AddWithValue("@FechaFin", x.FechaFin.ToString("yyyy-MM-dd"));
            cmd.Parameters.AddWithValue("@MontoMensual", x.MontoMensual);
            cmd.Parameters.AddWithValue("@Deposito", (object?)x.Deposito ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Estado", x.Estado ?? "Vigente");
            cmd.Parameters.AddWithValue("@Id", x.Id);
            return cmd.ExecuteNonQuery();
        }

        public int FinalizarContrato(int idContrato, int userId)
{
    using var conn = new SqliteConnection(cs);
    conn.Open();
    using var cmd = conn.CreateCommand();
    cmd.CommandText = @"
        UPDATE Contratos
        SET Estado = 'Finalizado',
            EndedByUserId = @UserId,
            EndedAt = datetime('now')
        WHERE Id=@Id;";
    cmd.Parameters.AddWithValue("@Id", idContrato);
    cmd.Parameters.AddWithValue("@UserId", userId);
    return cmd.ExecuteNonQuery();
}

        // === BAJA ===
        public int Baja(int id)
        {
            using var conn = new SqliteConnection(cs);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "DELETE FROM Contratos WHERE Id=@Id;";
            cmd.Parameters.AddWithValue("@Id", id);
            return cmd.ExecuteNonQuery();
        }

        // === VALIDAR SUPERPOSICIÓN ===
        public bool ExisteSuperposicion(int idInmueble, DateTime inicio, DateTime fin, int? idExcluido = null)
        {
            using var conn = new SqliteConnection(cs);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                SELECT COUNT(*)
                FROM Contratos
                WHERE IdInmueble=@IdInmueble
                AND ( (FechaInicio <= @Fin AND FechaFin >= @Inicio) )";
            if (idExcluido.HasValue)
                cmd.CommandText += " AND Id<>@IdExcluido";

            cmd.Parameters.AddWithValue("@IdInmueble", idInmueble);
            cmd.Parameters.AddWithValue("@Inicio", inicio.ToString("yyyy-MM-dd"));
            cmd.Parameters.AddWithValue("@Fin", fin.ToString("yyyy-MM-dd"));
            if (idExcluido.HasValue)
                cmd.Parameters.AddWithValue("@IdExcluido", idExcluido.Value);

            var count = Convert.ToInt32(cmd.ExecuteScalar());
            return count > 0;
        }
    }
}
