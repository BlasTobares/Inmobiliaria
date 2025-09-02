using Microsoft.AspNetCore.Mvc;
using Inmobiliaria.Data;
using Inmobiliaria.Models;

namespace Inmobiliaria.Controllers
{
    public class ContratosController : Controller
    {
        private readonly RepositorioContrato repo;
        private readonly RepositorioInmueble repoInm;
        private readonly RepositorioInquilino repoInq;

        public ContratosController(IConfiguration config)
        {
            var cs = config.GetConnectionString("DefaultConnection")!;
            repo = new RepositorioContrato(cs);
            repoInm = new RepositorioInmueble(cs);
            repoInq = new RepositorioInquilino(cs);
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
            ViewBag.Inmuebles = repoInm.ObtenerTodos();
            ViewBag.Inquilinos = repoInq.ObtenerTodos();
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Create(Contrato x)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Inmuebles = repoInm.ObtenerTodos();
                ViewBag.Inquilinos = repoInq.ObtenerTodos();
                return View(x);
            }
            repo.Alta(x);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int id)
        {
            var x = repo.ObtenerPorId(id);
            if (x == null) return NotFound();
            ViewBag.Inmuebles = repoInm.ObtenerTodos();
            ViewBag.Inquilinos = repoInq.ObtenerTodos();
            return View(x);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Contrato x)
        {
            if (id != x.Id) return BadRequest();
            if (!ModelState.IsValid)
            {
                ViewBag.Inmuebles = repoInm.ObtenerTodos();
                ViewBag.Inquilinos = repoInq.ObtenerTodos();
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
