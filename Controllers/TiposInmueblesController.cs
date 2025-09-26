using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Inmobiliaria.Data;
using Inmobiliaria.Models;

namespace Inmobiliaria.Controllers
{
    [Authorize]
    public class TiposInmueblesController : Controller
    {
        private readonly RepositorioTipoInmueble repo;

        public TiposInmueblesController(IConfiguration config)
        {
            repo = new RepositorioTipoInmueble(config.GetConnectionString("DefaultConnection")!);
        }

        public IActionResult Index() => View(repo.ObtenerTodos());

        public IActionResult Details(int id)
        {
            var t = repo.ObtenerPorId(id);
            if (t == null) return NotFound();
            return View(t);
        }

        public IActionResult Create() => View();

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Create(TipoInmueble t)
        {
            if (!ModelState.IsValid) return View(t);
            repo.Alta(t);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int id)
        {
            var t = repo.ObtenerPorId(id);
            if (t == null) return NotFound();
            return View(t);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Edit(TipoInmueble t)
        {
            if (!ModelState.IsValid) return View(t);
            repo.Modificacion(t);
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            var t = repo.ObtenerPorId(id);
            if (t == null) return NotFound();
            return View(t);
        }

        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken, Authorize(Roles = "Admin")]
        public IActionResult DeleteConfirmed(int id)
        {
            repo.Baja(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
