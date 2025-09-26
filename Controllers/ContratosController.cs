
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Inmobiliaria.Data;
using Inmobiliaria.Models;

namespace Inmobiliaria.Controllers
{
    [Authorize]
    public class ContratosController : Controller
    {
        private readonly RepositorioContrato repo;
        private readonly RepositorioInquilino repoInq;
        private readonly RepositorioInmueble repoInm;

        public ContratosController(IConfiguration config)
        {
            var cs = config.GetConnectionString("DefaultConnection")!;
            repo = new RepositorioContrato(cs);
            repoInq = new RepositorioInquilino(cs);
            repoInm = new RepositorioInmueble(cs);
        }

        // === INDEX ===
        public IActionResult Index() => View(repo.ObtenerTodos());

        // === DETAILS ===
        public IActionResult Details(int id)
        {
            var c = repo.ObtenerPorId(id);
            if (c == null) return NotFound();
            return View(c);
        }

        // === CREATE ===
        public IActionResult Create()
        {
            ViewBag.Inquilinos = repoInq.ObtenerTodos();
            ViewBag.Inmuebles = repoInm.ObtenerTodos();
            return View(new Contrato
            {
                FechaInicio = DateTime.Today,
                FechaFin = DateTime.Today.AddYears(1),
                Estado = "Vigente"
            });
        }
        [HttpPost, ValidateAntiForgeryToken]
public IActionResult Create(Contrato c)
{
    if (!ModelState.IsValid)
    {
        ViewBag.Inquilinos = repoInq.ObtenerTodos();
        ViewBag.Inmuebles = repoInm.ObtenerTodos();
        return View(c);
    }

    var inmueble = repoInm.ObtenerPorId(c.IdInmueble);
    if (inmueble == null)
    {
        ModelState.AddModelError("", "El inmueble seleccionado no existe.");
        ViewBag.Inquilinos = repoInq.ObtenerTodos();
        ViewBag.Inmuebles = repoInm.ObtenerTodos();
        return View(c);
    }

    if (!inmueble.Estado)
    {
        ModelState.AddModelError("", "El inmueble seleccionado no está disponible actualmente.");
        ViewBag.Inquilinos = repoInq.ObtenerTodos();
        ViewBag.Inmuebles = repoInm.ObtenerTodos();
        return View(c);
    }

    if (repo.ExisteSuperposicion(c.IdInmueble, c.FechaInicio, c.FechaFin))
    {
        ModelState.AddModelError("", "Ya existe un contrato para este inmueble en las fechas seleccionadas.");
        ViewBag.Inquilinos = repoInq.ObtenerTodos();
        ViewBag.Inmuebles = repoInm.ObtenerTodos();
        return View(c);
    }

    var hoy = DateTime.Today;
    /*if (repo.ExisteSuperposicion(c.IdInmueble, hoy, hoy))
    {
        ModelState.AddModelError("", "Este inmueble ya posee un contrato vigente para la fecha actual.");
        ViewBag.Inquilinos = repoInq.ObtenerTodos();
        ViewBag.Inmuebles = repoInm.ObtenerTodos();
        return View(c);
    }
*/
    // --- OBTENGO USERID desde SESSION (recomendado porque ya lo usás en Pagos)
    int? userId = HttpContext.Session.GetInt32("UserId"); // devuelve null si no existe

    // Llamo al repo pasando userId
    repo.Alta(c, userId);

    // Si entra en vigencia hoy, marco el inmueble no disponible
    if (c.FechaInicio <= hoy && c.FechaFin >= hoy)
    {
        inmueble.Estado = false;
        repoInm.Modificacion(inmueble);
    }

    return RedirectToAction(nameof(Index));
}

        // === EDIT ===
        public IActionResult Edit(int id)
        {
            var c = repo.ObtenerPorId(id);
            if (c == null) return NotFound();
            ViewBag.Inquilinos = repoInq.ObtenerTodos();
            ViewBag.Inmuebles = repoInm.ObtenerTodos();
            return View(c);
        }

[HttpPost, ValidateAntiForgeryToken]
public IActionResult Edit(Contrato c)
{
    if (!ModelState.IsValid)
    {
        ViewBag.Inquilinos = repoInq.ObtenerTodos();
        ViewBag.Inmuebles = repoInm.ObtenerTodos();
        return View(c);
    }

    if (repo.ExisteSuperposicion(c.IdInmueble, c.FechaInicio, c.FechaFin, c.Id))
    {
        ModelState.AddModelError("", "Ya existe otro contrato para este inmueble en las fechas seleccionadas.");
        ViewBag.Inquilinos = repoInq.ObtenerTodos();
        ViewBag.Inmuebles = repoInm.ObtenerTodos();
        return View(c);
    }

    repo.Modificacion(c);

    // 👇 Verificar vigencia y actualizar Estado del inmueble
    var inmueble = repoInm.ObtenerPorId(c.IdInmueble);
    if (inmueble != null)
    {
        var hoy = DateTime.Today;
        if (c.FechaInicio <= hoy && c.FechaFin >= hoy)
        {
            inmueble.Estado = false; // No disponible
            repoInm.Modificacion(inmueble);
        }
        else
        {
            inmueble.Estado = true; // Disponible nuevamente si no está vigente hoy
            repoInm.Modificacion(inmueble);
        }
    }

    return RedirectToAction(nameof(Index));
}

        
        // === DELETE ===
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            var c = repo.ObtenerPorId(id);
            if (c == null) return NotFound();
            return View(c);
        }

[HttpPost, ActionName("Delete")]
[ValidateAntiForgeryToken]
[Authorize(Roles = "Admin")]
public IActionResult DeleteConfirmed(int id)
{
    var userId = HttpContext.Session.GetInt32("UserId") ?? 0;
    repo.FinalizarContrato(id, userId);
    return RedirectToAction(nameof(Index));
}
/*

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteConfirmed(int id)
        {
            repo.Baja(id);
            return RedirectToAction(nameof(Index));
        }*/
    }
}
