
using Microsoft.Data.Sqlite;
using Inmobiliaria.Models;

namespace Inmobiliaria.Data
{
    public class RepositorioTipoInmueble
    {
        private readonly string connectionString;
        public RepositorioTipoInmueble(string connectionString) 
            => this.connectionString = connectionString;

        public List<TipoInmueble> ObtenerTodos()
        {
            var lista = new List<TipoInmueble>();
            using var conn = new SqliteConnection(connectionString);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT Id, Nombre, Descripcion 
                                FROM TiposInmuebles 
                                ORDER BY Nombre;";
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                lista.Add(new TipoInmueble
                {
                    Id = reader.GetInt32(0),
                    Nombre = reader.GetString(1),
                    Descripcion = reader.IsDBNull(2) ? null : reader.GetString(2)
                });
            }
            return lista;
        }

        public TipoInmueble? ObtenerPorId(int id)
        {
            using var conn = new SqliteConnection(connectionString);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT Id, Nombre, Descripcion 
                                FROM TiposInmuebles 
                                WHERE Id=@Id;";
            cmd.Parameters.AddWithValue("@Id", id);
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new TipoInmueble
                {
                    Id = reader.GetInt32(0),
                    Nombre = reader.GetString(1),
                    Descripcion = reader.IsDBNull(2) ? null : reader.GetString(2)
                };
            }
            return null;
        }

        public int Alta(TipoInmueble t)
        {
            using var conn = new SqliteConnection(connectionString);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"INSERT INTO TiposInmuebles (Nombre, Descripcion)
                                VALUES (@Nombre, @Descripcion);
                                SELECT last_insert_rowid();";
            cmd.Parameters.AddWithValue("@Nombre", t.Nombre);
            cmd.Parameters.AddWithValue("@Descripcion", (object?)t.Descripcion ?? DBNull.Value);
            var id = Convert.ToInt32(cmd.ExecuteScalar());
            t.Id = id;
            return id;
        }

        public int Modificacion(TipoInmueble t)
        {
            using var conn = new SqliteConnection(connectionString);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"UPDATE TiposInmuebles
                                SET Nombre=@Nombre, Descripcion=@Descripcion
                                WHERE Id=@Id;";
            cmd.Parameters.AddWithValue("@Nombre", t.Nombre);
            cmd.Parameters.AddWithValue("@Descripcion", (object?)t.Descripcion ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Id", t.Id);
            return cmd.ExecuteNonQuery();
        }

        public int Baja(int id)
        {
            using var conn = new SqliteConnection(connectionString);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"DELETE FROM TiposInmuebles WHERE Id=@Id;";
            cmd.Parameters.AddWithValue("@Id", id);
            return cmd.ExecuteNonQuery();
        }
    }
}
