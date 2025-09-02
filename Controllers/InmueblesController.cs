using Microsoft.AspNetCore.Mvc;
using Inmobiliaria.Data;
using Inmobiliaria.Models;

namespace Inmobiliaria.Controllers
{
    public class InmueblesController : Controller
    {
        private readonly RepositorioInmueble repo;
        private readonly RepositorioPropietario repoProp;

        public InmueblesController(IConfiguration config)
        {
            var cs = config.GetConnectionString("DefaultConnection")!;
            repo = new RepositorioInmueble(cs);
            repoProp = new RepositorioPropietario(cs);
        }

        public IActionResult Index()
        {
            var lista = repo.ObtenerTodos();
            return View(lista);
        }

        public IActionResult Details(int id)
        {
            var x = repo.ObtenerPorId(id);
            if (x == null) return NotFound();
            return View(x);
        }

        public IActionResult Create()
        {
            ViewBag.Propietarios = repoProp.ObtenerTodos();
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Create(Inmueble x)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Propietarios = repoProp.ObtenerTodos();
                return View(x);
            }
            repo.Alta(x);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int id)
        {
            var x = repo.ObtenerPorId(id);
            if (x == null) return NotFound();
            ViewBag.Propietarios = repoProp.ObtenerTodos();
            return View(x);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Inmueble x)
        {
            if (id != x.Id) return BadRequest();
            if (!ModelState.IsValid)
            {
                ViewBag.Propietarios = repoProp.ObtenerTodos();
                return View(x);
            }
            repo.Modificacion(x);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int id)
        {
            var x = repo.ObtenerPorId(id);
            if (x == null) return NotFound();
            return View(x);
        }

        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            repo.Baja(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
