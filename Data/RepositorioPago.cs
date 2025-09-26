
using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using Inmobiliaria.Models;

namespace Inmobiliaria.Data
{
    public class RepositorioPago
    {
        private readonly string connectionString;
        public RepositorioPago(string connectionString) => this.connectionString = connectionString;

        public List<Pago> ObtenerTodos()
        {
            var lista = new List<Pago>();
            using var conn = new SqliteConnection(connectionString);
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                SELECT p.Id, p.IdContrato, p.NroPago, p.Fecha, p.Importe, p.Detalle, p.Estado,
                       p.CreatedByUserId, p.CreatedAt, p.AnnulledByUserId, p.AnnulledAt,
                       c.Id, i.Direccion || ' - ' || inq.NombreCompleto as ContratoInfo
                FROM Pagos p
                JOIN Contratos c ON c.Id = p.IdContrato
                JOIN Inmuebles i ON i.Id = c.IdInmueble
                JOIN Inquilinos inq ON inq.Id = c.IdInquilino
                ORDER BY p.Fecha DESC;";
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                lista.Add(new Pago
                {
                    Id = reader.GetInt32(0),
                    IdContrato = reader.GetInt32(1),
                    NroPago = reader.GetInt32(2),
                    Fecha = DateTime.Parse(reader.GetString(3)),
                    Importe = reader.GetDouble(4),
                    Detalle = reader.IsDBNull(5) ? null : reader.GetString(5),
                    Estado = reader.GetString(6),
                    CreatedByUserId = reader.IsDBNull(7) ? null : reader.GetInt32(7),
                    CreatedAt = reader.IsDBNull(8) ? null : DateTime.Parse(reader.GetString(8)),
                    AnnulledByUserId = reader.IsDBNull(9) ? null : reader.GetInt32(9),
                    AnnulledAt = reader.IsDBNull(10) ? null : DateTime.Parse(reader.GetString(10)),
                    ContratoInfo = reader.IsDBNull(11) ? "" : reader.GetString(11)
                });
            }
            return lista;
        }

        // NUEVO: obtener pagos por contrato (todos)
        public List<Pago> ObtenerPorContrato(int contratoId)
        {
            var lista = new List<Pago>();
            using var conn = new SqliteConnection(connectionString);
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                SELECT p.Id, p.IdContrato, p.NroPago, p.Fecha, p.Importe, p.Detalle, p.Estado,
                       p.CreatedByUserId, p.CreatedAt, p.AnnulledByUserId, p.AnnulledAt
                FROM Pagos p
                WHERE p.IdContrato=@IdContrato
                ORDER BY p.NroPago;";
            cmd.Parameters.AddWithValue("@IdContrato", contratoId);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                lista.Add(new Pago
                {
                    Id = reader.GetInt32(0),
                    IdContrato = reader.GetInt32(1),
                    NroPago = reader.GetInt32(2),
                    Fecha = DateTime.Parse(reader.GetString(3)),
                    Importe = reader.GetDouble(4),
                    Detalle = reader.IsDBNull(5) ? null : reader.GetString(5),
                    Estado = reader.GetString(6),
                    CreatedByUserId = reader.IsDBNull(7) ? null : reader.GetInt32(7),
                    CreatedAt = reader.IsDBNull(8) ? null : DateTime.Parse(reader.GetString(8)),
                    AnnulledByUserId = reader.IsDBNull(9) ? null : reader.GetInt32(9),
                    AnnulledAt = reader.IsDBNull(10) ? null : DateTime.Parse(reader.GetString(10))
                });
            }
            return lista;
        }

        public Pago? ObtenerPorId(int id)
        {
            using var conn = new SqliteConnection(connectionString);
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                SELECT p.Id, p.IdContrato, p.NroPago, p.Fecha, p.Importe, p.Detalle, p.Estado,
                       p.CreatedByUserId, p.CreatedAt, p.AnnulledByUserId, p.AnnulledAt,
                       c.Id, i.Direccion || ' - ' || inq.NombreCompleto as ContratoInfo
                FROM Pagos p
                JOIN Contratos c ON c.Id = p.IdContrato
                JOIN Inmuebles i ON i.Id = c.IdInmueble
                JOIN Inquilinos inq ON inq.Id = c.IdInquilino
                WHERE p.Id=@Id;";
            cmd.Parameters.AddWithValue("@Id", id);
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new Pago
                {
                    Id = reader.GetInt32(0),
                    IdContrato = reader.GetInt32(1),
                    NroPago = reader.GetInt32(2),
                    Fecha = DateTime.Parse(reader.GetString(3)),
                    Importe = reader.GetDouble(4),
                    Detalle = reader.IsDBNull(5) ? null : reader.GetString(5),
                    Estado = reader.GetString(6),
                    CreatedByUserId = reader.IsDBNull(7) ? null : reader.GetInt32(7),
                    CreatedAt = reader.IsDBNull(8) ? null : DateTime.Parse(reader.GetString(8)),
                    AnnulledByUserId = reader.IsDBNull(9) ? null : reader.GetInt32(9),
                    AnnulledAt = reader.IsDBNull(10) ? null : DateTime.Parse(reader.GetString(10)),
                    ContratoInfo = reader.IsDBNull(11) ? "" : reader.GetString(11)
                };
            }
            return null;
        }

        public int Alta(Pago p, int userId)
        {
            using var conn = new SqliteConnection(connectionString);
            conn.Open();

            // Buscar último número de pago del contrato
            var cmdMax = conn.CreateCommand();
            cmdMax.CommandText = "SELECT IFNULL(MAX(NroPago), 0) FROM Pagos WHERE IdContrato = @IdContrato";
            cmdMax.Parameters.AddWithValue("@IdContrato", p.IdContrato);
            var ultimoNro = Convert.ToInt32(cmdMax.ExecuteScalar());

            p.NroPago = ultimoNro + 1; // asignamos nro de pago incremental

            var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                INSERT INTO Pagos (IdContrato, NroPago, Fecha, Importe, Detalle, Estado, CreatedByUserId, CreatedAt)
                VALUES (@IdContrato, @NroPago, @Fecha, @Importe, @Detalle, 'Activo', @UserId, datetime('now'));
                SELECT last_insert_rowid();";
            cmd.Parameters.AddWithValue("@IdContrato", p.IdContrato);
            cmd.Parameters.AddWithValue("@NroPago", p.NroPago);
            cmd.Parameters.AddWithValue("@Fecha", p.Fecha.ToString("yyyy-MM-dd"));
            cmd.Parameters.AddWithValue("@Importe", p.Importe);
            cmd.Parameters.AddWithValue("@Detalle", (object?)p.Detalle ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@UserId", userId);

            var id = Convert.ToInt32(cmd.ExecuteScalar());
            p.Id = id;
            return id;
        }

        public int Anular(int id, int userId)
        {
            using var conn = new SqliteConnection(connectionString);
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = @"UPDATE Pagos 
                                SET Estado='Anulado', AnnulledByUserId=@UserId, AnnulledAt=datetime('now') 
                                WHERE Id=@Id;";
            cmd.Parameters.AddWithValue("@UserId", userId);
            cmd.Parameters.AddWithValue("@Id", id);
            return cmd.ExecuteNonQuery();
        }
    }
}
