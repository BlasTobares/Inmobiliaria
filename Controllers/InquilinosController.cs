using Microsoft.AspNetCore.Mvc;
using Inmobiliaria.Data;
using Inmobiliaria.Models;

namespace Inmobiliaria.Controllers
{
    public class InquilinosController : Controller
    {
        private readonly RepositorioInquilino repo;

        public InquilinosController(IConfiguration config)
        {
            var cs = config.GetConnectionString("DefaultConnection");
            repo = new RepositorioInquilino(cs!);
        }

        public IActionResult Index()
        {
            var lista = repo.ObtenerTodos();
            return View(lista);
        }

        public IActionResult Details(int id)
        {
            var i = repo.ObtenerPorId(id);
            if (i == null) return NotFound();
            return View(i);
        }

        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Inquilino i)
        {
            if (!ModelState.IsValid) return View(i);
            repo.Alta(i);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int id)
        {
            var i = repo.ObtenerPorId(id);
            if (i == null) return NotFound();
            return View(i);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Inquilino i)
        {
            if (id != i.Id) return BadRequest();
            if (!ModelState.IsValid) return View(i);
            repo.Modificacion(i);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int id)
        {
            var i = repo.ObtenerPorId(id);
            if (i == null) return NotFound();
            return View(i);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            repo.Baja(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
