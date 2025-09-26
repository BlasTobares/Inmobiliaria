
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Inmobiliaria.Data;
using Inmobiliaria.Models;

namespace Inmobiliaria.Controllers
{
    [Authorize]
    public class InmueblesController : Controller
    {
        private readonly RepositorioInmueble repo;
        private readonly RepositorioPropietario repoProp;
        private readonly RepositorioTipoInmueble repoTipo;

        public InmueblesController(IConfiguration config)
        {
            var cs = config.GetConnectionString("DefaultConnection")!;
            repo = new RepositorioInmueble(cs);
            repoProp = new RepositorioPropietario(cs);
            repoTipo = new RepositorioTipoInmueble(cs);
        }

        // INDEX
        public IActionResult Index()
        {
            var lista = repo.ObtenerTodos();
            return View(lista);
        }

        // DETAILS
        public IActionResult Details(int id)
        {
            var i = repo.ObtenerPorId(id);
            if (i == null) return NotFound();
            return View(i);
        }

        // GET: Inmuebles/Create
public IActionResult Create()
{
    var repoPropietarios = new RepositorioPropietario(
        HttpContext.RequestServices.GetService<IConfiguration>()!.GetConnectionString("DefaultConnection")!
    );
    var repoTipos = new RepositorioTipoInmueble(
        HttpContext.RequestServices.GetService<IConfiguration>()!.GetConnectionString("DefaultConnection")!
    );

    ViewBag.Propietarios = repoPropietarios.ObtenerTodos();
    ViewBag.TiposInmuebles = repoTipos.ObtenerTodos();

    return View();
}

[HttpPost]
[ValidateAntiForgeryToken]
public IActionResult Create(Inmueble i)
{
    if (!ModelState.IsValid)
    {
        var repoPropietarios = new RepositorioPropietario(
            HttpContext.RequestServices.GetService<IConfiguration>()!.GetConnectionString("DefaultConnection")!
        );
        var repoTipos = new RepositorioTipoInmueble(
            HttpContext.RequestServices.GetService<IConfiguration>()!.GetConnectionString("DefaultConnection")!
        );

        ViewBag.Propietarios = repoPropietarios.ObtenerTodos();
        ViewBag.TiposInmuebles = repoTipos.ObtenerTodos();

        return View(i);
    }

    repo.Alta(i);
    return RedirectToAction(nameof(Index));
}

        // EDIT (GET)
        public IActionResult Edit(int id)
        {
            var i = repo.ObtenerPorId(id);
            if (i == null) return NotFound();

            ViewBag.Propietarios = repoProp.ObtenerTodos();
            ViewBag.Tipos = repoTipo.ObtenerTodos();
            return View(i);
        }

        // EDIT (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Inmueble i)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Propietarios = repoProp.ObtenerTodos();
                ViewBag.Tipos = repoTipo.ObtenerTodos();
                return View(i);
            }

            repo.Modificacion(i);
            return RedirectToAction(nameof(Index));
        }
public IActionResult Disponibles()
{
    var lista = repo.ObtenerDisponibles();
    return View(lista);
}

        // DELETE (GET) - solo Admin puede borrar
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            var i = repo.ObtenerPorId(id);
            if (i == null) return NotFound();
            return View(i);
        }

        // GET: muestra formulario para buscar inmuebles libres por fechas
public IActionResult DisponiblesPorFecha()
{
    ViewBag.FechaInicio = DateTime.Today;
    ViewBag.FechaFin = DateTime.Today.AddMonths(1);
    return View(new List<Inmobiliaria.Models.Inmueble>());
}

// POST: recibe fechas y muestra inmuebles no ocupados en ese intervalo
[HttpPost]
[ValidateAntiForgeryToken]
public IActionResult DisponiblesPorFecha(DateTime fechaInicio, DateTime fechaFin)
{
    if (fechaInicio > fechaFin)
    {
        ModelState.AddModelError("", "La fecha 'Desde' no puede ser mayor que la fecha 'Hasta'.");
        ViewBag.FechaInicio = fechaInicio;
        ViewBag.FechaFin = fechaFin;
        return View(new List<Inmobiliaria.Models.Inmueble>());
    }

    var lista = repo.ObtenerNoOcupadosEntre(fechaInicio, fechaFin);
    ViewBag.FechaInicio = fechaInicio;
    ViewBag.FechaFin = fechaFin;
    return View(lista);
}


        // DELETE (POST)
        [HttpPost, ActionName("Delete")]
[ValidateAntiForgeryToken]
[Authorize(Roles = "Admin")]
public IActionResult DeleteConfirmed(int id)
{
    try
    {
        repo.Baja(id);
        TempData["Success"] = "Inmueble eliminado correctamente.";
    }
    catch (InvalidOperationException ex)
    {
        // Mensaje amigable para el usuario
        TempData["Error"] = ex.Message;
    }
    return RedirectToAction(nameof(Index));
}

    }
}
