using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using Inmobiliaria.Models;

namespace Inmobiliaria.Data
{
    public class RepositorioPropietario
    {
        private readonly string connectionString;
        public RepositorioPropietario(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public List<Propietario> ObtenerTodos()
        {
            var lista = new List<Propietario>();
            using var conn = new SqliteConnection(connectionString);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT Id, DNI, Apellido, Nombre, Telefono, Email
                                FROM Propietarios
                                ORDER BY Apellido, Nombre;";
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                lista.Add(new Propietario
                {
                    Id = reader.GetInt32(0),
                    DNI = reader.IsDBNull(1) ? "" : reader.GetString(1),
                    Apellido = reader.IsDBNull(2) ? "" : reader.GetString(2),
                    Nombre = reader.IsDBNull(3) ? "" : reader.GetString(3),
                    Telefono = reader.IsDBNull(4) ? "" : reader.GetString(4),
                    Email = reader.IsDBNull(5) ? "" : reader.GetString(5),
                });
            }
            return lista;
        }

        public Propietario? ObtenerPorId(int id)
        {
            using var conn = new SqliteConnection(connectionString);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT Id, DNI, Apellido, Nombre, Telefono, Email
                                FROM Propietarios WHERE Id=@Id;";
            cmd.Parameters.AddWithValue("@Id", id);
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new Propietario
                {
                    Id = reader.GetInt32(0),
                    DNI = reader.IsDBNull(1) ? "" : reader.GetString(1),
                    Apellido = reader.IsDBNull(2) ? "" : reader.GetString(2),
                    Nombre = reader.IsDBNull(3) ? "" : reader.GetString(3),
                    Telefono = reader.IsDBNull(4) ? "" : reader.GetString(4),
                    Email = reader.IsDBNull(5) ? "" : reader.GetString(5),
                };
            }
            return null;
        }

        public int Alta(Propietario p)
        {
            using var conn = new SqliteConnection(connectionString);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"INSERT INTO Propietarios (DNI, Apellido, Nombre, Telefono, Email)
                                VALUES (@DNI, @Apellido, @Nombre, @Telefono, @Email);
                                SELECT last_insert_rowid();";
            cmd.Parameters.AddWithValue("@DNI", p.DNI ?? "");
            cmd.Parameters.AddWithValue("@Apellido", p.Apellido ?? "");
            cmd.Parameters.AddWithValue("@Nombre", p.Nombre ?? "");
            cmd.Parameters.AddWithValue("@Telefono", (object?)p.Telefono ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Email", (object?)p.Email ?? DBNull.Value);
            var id = Convert.ToInt32(cmd.ExecuteScalar());
            p.Id = id;
            return id;
        }

        public int Modificacion(Propietario p)
        {
            using var conn = new SqliteConnection(connectionString);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"UPDATE Propietarios
                                SET DNI=@DNI, Apellido=@Apellido, Nombre=@Nombre, Telefono=@Telefono, Email=@Email
                                WHERE Id=@Id;";
            cmd.Parameters.AddWithValue("@DNI", p.DNI ?? "");
            cmd.Parameters.AddWithValue("@Apellido", p.Apellido ?? "");
            cmd.Parameters.AddWithValue("@Nombre", p.Nombre ?? "");
            cmd.Parameters.AddWithValue("@Telefono", (object?)p.Telefono ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Email", (object?)p.Email ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Id", p.Id);
            return cmd.ExecuteNonQuery();
        }

        public int Baja(int id)
        {
            using var conn = new SqliteConnection(connectionString);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"DELETE FROM Propietarios WHERE Id=@Id;";
            cmd.Parameters.AddWithValue("@Id", id);
            return cmd.ExecuteNonQuery();
        }
    }
}