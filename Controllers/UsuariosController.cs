
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.IO;
using Inmobiliaria.Data;
using Inmobiliaria.Models;

namespace Inmobiliaria.Controllers
{
    [Authorize] // todos los endpoints requieren autenticación por defecto
    public class UsuariosController : Controller
    {
        private readonly RepositorioUsuario repo;

        public UsuariosController(IConfiguration config)
        {
            repo = new RepositorioUsuario(config.GetConnectionString("DefaultConnection")!);
        }

        // === ADMIN: listar / ABM usuarios ===
        [Authorize(Roles = "Admin")]
        public IActionResult Index() => View(repo.ObtenerTodos());

        [Authorize(Roles = "Admin")]
        public IActionResult Details(int id)
        {
            var u = repo.ObtenerPorId(id);
            if (u == null) return NotFound();
            return View(u);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create() => View();

        // ahora recibimos avatarFile y guardamos el archivo después de crear al usuario
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Usuario u, string password, IFormFile? avatarFile)
        {
            if (!ModelState.IsValid) return View(u);

            // Crear usuario (inserta y asigna Id a 'u')
            var id = repo.Crear(u, password);

            // Procesar avatar si llega
            if (avatarFile != null && avatarFile.Length > 0)
            {
                var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "avatars");
                if (!Directory.Exists(uploads)) Directory.CreateDirectory(uploads);

                var fileName = $"{id}_{Path.GetFileName(avatarFile.FileName)}";
                var filePath = Path.Combine(uploads, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    avatarFile.CopyTo(stream);
                }

                u.Id = id;
                u.AvatarPath = "/avatars/" + fileName;
                repo.Modificar(u); // actualizamos AvatarPath
            }

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Edit(int id)
        {
            var u = repo.ObtenerPorId(id);
            if (u == null) return NotFound();
            return View(u);
        }

        [HttpPost]
[Authorize(Roles = "Admin")]
[ValidateAntiForgeryToken]
public IActionResult Edit(Usuario u, IFormFile? avatarFile, string? removeAvatar)
{
    var usuario = repo.ObtenerPorId(u.Id);
    if (usuario == null) return NotFound();

    // Actualizar datos básicos
    usuario.Nombre = u.Nombre;
    usuario.Apellido = u.Apellido;
    usuario.Rol = u.Rol;

    // Eliminar avatar
    if (!string.IsNullOrEmpty(removeAvatar) && !string.IsNullOrEmpty(usuario.AvatarPath))
    {
        var rutaCompleta = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", usuario.AvatarPath.TrimStart('/'));
        if (System.IO.File.Exists(rutaCompleta))
            System.IO.File.Delete(rutaCompleta);
        usuario.AvatarPath = null;
    }

    // Subir avatar nuevo
    if (avatarFile != null && avatarFile.Length > 0)
    {
        var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "avatars");
        if (!Directory.Exists(uploads)) Directory.CreateDirectory(uploads);

        var fileName = $"{usuario.Id}_{Path.GetFileName(avatarFile.FileName)}";
        var filePath = Path.Combine(uploads, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            avatarFile.CopyTo(stream);
        }

        usuario.AvatarPath = "/avatars/" + fileName;
    }

    repo.Modificar(usuario);

    return RedirectToAction(nameof(Index));
}


        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            var u = repo.ObtenerPorId(id);
            if (u == null) return NotFound();
            return View(u);
        }

        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            repo.Borrar(id);
            return RedirectToAction(nameof(Index));
        }

        // === PERFIL === (cualquier usuario autenticado puede ver/editar su perfil)
        [Authorize] // override del requerimiento de rol (class-level es [Authorize], los métodos marcados con Roles = "Admin" son los que restringen)
        public IActionResult Perfil()
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var u = repo.ObtenerPorId(userId);
            if (u == null) return NotFound();
            return View(u);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public IActionResult Perfil(Usuario u, string? nuevaPassword, IFormFile? avatarFile, string? removeAvatar)
        {
            var usuario = repo.ObtenerPorId(u.Id);
            if (usuario == null) return NotFound();

            // Actualizar datos básicos
            usuario.Nombre = u.Nombre;
            usuario.Apellido = u.Apellido;

            // Eliminar avatar (si se solicitó)
            if (!string.IsNullOrEmpty(removeAvatar) && !string.IsNullOrEmpty(usuario.AvatarPath))
            {
                var rutaCompleta = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", usuario.AvatarPath.TrimStart('/'));
                if (System.IO.File.Exists(rutaCompleta))
                    System.IO.File.Delete(rutaCompleta);
                usuario.AvatarPath = null;
            }

            // Subir avatar nuevo
            if (avatarFile != null && avatarFile.Length > 0)
            {
                var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "avatars");
                if (!Directory.Exists(uploads)) Directory.CreateDirectory(uploads);

                var fileName = $"{usuario.Id}_{Path.GetFileName(avatarFile.FileName)}";
                var filePath = Path.Combine(uploads, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    avatarFile.CopyTo(stream);
                }

                usuario.AvatarPath = "/avatars/" + fileName;
            }

            // Cambiar contraseña si corresponde
            if (!string.IsNullOrEmpty(nuevaPassword))
                repo.CambiarPassword(usuario.Id, nuevaPassword);

            repo.Modificar(usuario);

            TempData["Msg"] = "Perfil actualizado correctamente";
            return RedirectToAction(nameof(Perfil));
        }
    }
}
