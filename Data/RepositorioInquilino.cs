using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using Inmobiliaria.Models;

namespace Inmobiliaria.Data
{
    public class RepositorioInquilino
    {
        private readonly string connectionString;
        public RepositorioInquilino(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public List<Inquilino> ObtenerTodos()
        {
            var lista = new List<Inquilino>();
            using var conn = new SqliteConnection(connectionString);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT Id, DNI, NombreCompleto, Telefono, Email
                                FROM Inquilinos
                                ORDER BY NombreCompleto;";
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                lista.Add(new Inquilino
                {
                    Id = reader.GetInt32(0),
                    DNI = reader.IsDBNull(1) ? "" : reader.GetString(1),
                    NombreCompleto = reader.IsDBNull(2) ? "" : reader.GetString(2),
                    Telefono = reader.IsDBNull(3) ? "" : reader.GetString(3),
                    Email = reader.IsDBNull(4) ? "" : reader.GetString(4),
                });
            }
            return lista;
        }

        public Inquilino? ObtenerPorId(int id)
        {
            using var conn = new SqliteConnection(connectionString);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT Id, DNI, NombreCompleto, Telefono, Email
                                FROM Inquilinos WHERE Id=@Id;";
            cmd.Parameters.AddWithValue("@Id", id);
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new Inquilino
                {
                    Id = reader.GetInt32(0),
                    DNI = reader.IsDBNull(1) ? "" : reader.GetString(1),
                    NombreCompleto = reader.IsDBNull(2) ? "" : reader.GetString(2),
                    Telefono = reader.IsDBNull(3) ? "" : reader.GetString(3),
                    Email = reader.IsDBNull(4) ? "" : reader.GetString(4),
                };
            }
            return null;
        }

        public int Alta(Inquilino i)
        {
            using var conn = new SqliteConnection(connectionString);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"INSERT INTO Inquilinos (DNI, NombreCompleto, Telefono, Email)
                                VALUES (@DNI, @NombreCompleto, @Telefono, @Email);
                                SELECT last_insert_rowid();";
            cmd.Parameters.AddWithValue("@DNI", i.DNI ?? "");
            cmd.Parameters.AddWithValue("@NombreCompleto", i.NombreCompleto ?? "");
            cmd.Parameters.AddWithValue("@Telefono", (object?)i.Telefono ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Email", (object?)i.Email ?? DBNull.Value);
            var id = Convert.ToInt32(cmd.ExecuteScalar());
            i.Id = id;
            return id;
        }

        public int Modificacion(Inquilino i)
        {
            using var conn = new SqliteConnection(connectionString);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"UPDATE Inquilinos
                                SET DNI=@DNI, NombreCompleto=@NombreCompleto, Telefono=@Telefono, Email=@Email
                                WHERE Id=@Id;";
            cmd.Parameters.AddWithValue("@DNI", i.DNI ?? "");
            cmd.Parameters.AddWithValue("@NombreCompleto", i.NombreCompleto ?? "");
            cmd.Parameters.AddWithValue("@Telefono", (object?)i.Telefono ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Email", (object?)i.Email ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Id", i.Id);
            return cmd.ExecuteNonQuery();
        }

        public int Baja(int id)
        {
            using var conn = new SqliteConnection(connectionString);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"DELETE FROM Inquilinos WHERE Id=@Id;";
            cmd.Parameters.AddWithValue("@Id", id);
            return cmd.ExecuteNonQuery();
        }
    }
}