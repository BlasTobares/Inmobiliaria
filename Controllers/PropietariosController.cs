using Microsoft.AspNetCore.Mvc;
using Inmobiliaria.Data;
using Inmobiliaria.Models;

namespace Inmobiliaria.Controllers
{
    public class PropietariosController : Controller
    {
        private readonly RepositorioPropietario repo;

        public PropietariosController(IConfiguration config)
        {
            var cs = config.GetConnectionString("DefaultConnection");
            repo = new RepositorioPropietario(cs!);
        }

        // GET: /Propietarios
        public IActionResult Index()
        {
            var lista = repo.ObtenerTodos();
            return View(lista);
        }

        // GET: /Propietarios/Details/5
        public IActionResult Details(int id)
        {
            var p = repo.ObtenerPorId(id);
            if (p == null) return NotFound();
            return View(p);
        }

        // GET: /Propietarios/Create
        public IActionResult Create() => View();

        // POST: /Propietarios/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Propietario p)
        {
            if (!ModelState.IsValid) return View(p);
            repo.Alta(p);
            return RedirectToAction(nameof(Index));
        }

        // GET: /Propietarios/Edit/5
        public IActionResult Edit(int id)
        {
            var p = repo.ObtenerPorId(id);
            if (p == null) return NotFound();
            return View(p);
        }

        // POST: /Propietarios/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Propietario p)
        {
            if (id != p.Id) return BadRequest();
            if (!ModelState.IsValid) return View(p);
            repo.Modificacion(p);
            return RedirectToAction(nameof(Index));
        }

        // GET: /Propietarios/Delete/5
        public IActionResult Delete(int id)
        {
            var p = repo.ObtenerPorId(id);
            if (p == null) return NotFound();
            return View(p);
        }

        // POST: /Propietarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            repo.Baja(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
