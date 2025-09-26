using Microsoft.Data.Sqlite;
using Inmobiliaria.Models;

namespace Inmobiliaria.Data
{
    public class RepositorioUsuario
    {
        private readonly string connectionString;

        public RepositorioUsuario(string connectionString)
        {
            this.connectionString = connectionString;
        }

        // === Helpers ===
        private string HashPassword(string password)
        {
            // Hash simple (para demo académica). 
            // En un sistema real usá bcrypt o Argon2.
            return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(password));
        }

        // === CRUD ===
        public Usuario? ObtenerPorId(int id)
        {
            Usuario? u = null;
            using var conn = new SqliteConnection(connectionString);
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT Id, Email, Nombre, Apellido, PasswordHash, Rol, AvatarPath 
                                FROM Usuarios WHERE Id=@Id;";
            cmd.Parameters.AddWithValue("@Id", id);
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                u = new Usuario
                {
                    Id = reader.GetInt32(0),
                    Email = reader.GetString(1),
                    Nombre = reader.GetString(2),
                    Apellido = reader.IsDBNull(3) ? "" : reader.GetString(3),
                    PasswordHash = reader.GetString(4),
                    Rol = reader.GetString(5),
                    AvatarPath = reader.IsDBNull(6) ? null : reader.GetString(6),
                };
            }
            return u;
        }

        public Usuario? ObtenerPorEmail(string email)
        {
            Usuario? u = null;
            using var conn = new SqliteConnection(connectionString);
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT Id, Email, Nombre, Apellido, PasswordHash, Rol, AvatarPath 
                                FROM Usuarios WHERE Email=@Email;";
            cmd.Parameters.AddWithValue("@Email", email);
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                u = new Usuario
                {
                    Id = reader.GetInt32(0),
                    Email = reader.GetString(1),
                    Nombre = reader.GetString(2),
                    Apellido = reader.IsDBNull(3) ? "" : reader.GetString(3),
                    PasswordHash = reader.GetString(4),
                    Rol = reader.GetString(5),
                    AvatarPath = reader.IsDBNull(6) ? null : reader.GetString(6),
                };
            }
            return u;
        }

        public int Crear(Usuario u, string password)
        {
            using var conn = new SqliteConnection(connectionString);
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = @"INSERT INTO Usuarios 
                                (Email, Nombre, Apellido, PasswordHash, Rol, AvatarPath, CreatedAt)
                                VALUES (@Email, @Nombre, @Apellido, @PasswordHash, @Rol, @AvatarPath, datetime('now'));
                                SELECT last_insert_rowid();";
            cmd.Parameters.AddWithValue("@Email", u.Email);
            cmd.Parameters.AddWithValue("@Nombre", u.Nombre);
            cmd.Parameters.AddWithValue("@Apellido", u.Apellido ?? "");
            cmd.Parameters.AddWithValue("@PasswordHash", HashPassword(password));
            cmd.Parameters.AddWithValue("@Rol", u.Rol);
            cmd.Parameters.AddWithValue("@AvatarPath", (object?)u.AvatarPath ?? DBNull.Value);

            var id = Convert.ToInt32(cmd.ExecuteScalar());
            u.Id = id;
            return id;
        }

        public int Modificar(Usuario u)
        {
            using var conn = new SqliteConnection(connectionString);
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = @"UPDATE Usuarios 
                                SET Nombre=@Nombre, Apellido=@Apellido, Rol=@Rol, AvatarPath=@AvatarPath
                                WHERE Id=@Id;";
            cmd.Parameters.AddWithValue("@Nombre", u.Nombre);
            cmd.Parameters.AddWithValue("@Apellido", u.Apellido ?? "");
            cmd.Parameters.AddWithValue("@Rol", u.Rol);
            cmd.Parameters.AddWithValue("@AvatarPath", (object?)u.AvatarPath ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Id", u.Id);
            return cmd.ExecuteNonQuery();
        }

        public int CambiarPassword(int id, string nuevaPassword)
        {
            using var conn = new SqliteConnection(connectionString);
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = @"UPDATE Usuarios 
                                SET PasswordHash=@PasswordHash 
                                WHERE Id=@Id;";
            cmd.Parameters.AddWithValue("@PasswordHash", HashPassword(nuevaPassword));
            cmd.Parameters.AddWithValue("@Id", id);
            return cmd.ExecuteNonQuery();
        }

        public int Borrar(int id)
        {
            using var conn = new SqliteConnection(connectionString);
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = @"DELETE FROM Usuarios WHERE Id=@Id;";
            cmd.Parameters.AddWithValue("@Id", id);
            return cmd.ExecuteNonQuery();
        }

        public List<Usuario> ObtenerTodos()
        {
            var lista = new List<Usuario>();
            using var conn = new SqliteConnection(connectionString);
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT Id, Email, Nombre, Apellido, Rol, AvatarPath 
                                FROM Usuarios ORDER BY Apellido, Nombre;";
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                lista.Add(new Usuario
                {
                    Id = reader.GetInt32(0),
                    Email = reader.GetString(1),
                    Nombre = reader.GetString(2),
                    Apellido = reader.IsDBNull(3) ? "" : reader.GetString(3),
                    Rol = reader.GetString(4),
                    AvatarPath = reader.IsDBNull(5) ? null : reader.GetString(5),
                });
            }
            return lista;
        }
    }
}

