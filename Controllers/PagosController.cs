
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Inmobiliaria.Data;
using Inmobiliaria.Models;
using System;

namespace Inmobiliaria.Controllers
{
    [Authorize]
    public class PagosController : Controller
    {
        private readonly RepositorioPago repo;
        private readonly RepositorioContrato repoContrato;
        private readonly RepositorioUsuario repoUsuario;

        public PagosController(IConfiguration config)
        {
            var cs = config.GetConnectionString("DefaultConnection")!;
            repo = new RepositorioPago(cs);
            repoContrato = new RepositorioContrato(cs);
            repoUsuario = new RepositorioUsuario(cs);
        }

        public IActionResult Index(int? contratoId = null)
        {
            if (contratoId.HasValue)
                return View(repo.ObtenerPorContrato(contratoId.Value));
            return View(repo.ObtenerTodos());
        }

        public IActionResult Details(int id)
        {
            var p = repo.ObtenerPorId(id);
            if (p == null) return NotFound();

            // resolvemos nombres para auditoría (si existen)
            ViewBag.CreatedByName = p.CreatedByUserId.HasValue ? FormatoUsuario(repoUsuario.ObtenerPorId(p.CreatedByUserId.Value)) : "-";
            ViewBag.AnnulledByName = p.AnnulledByUserId.HasValue ? FormatoUsuario(repoUsuario.ObtenerPorId(p.AnnulledByUserId.Value)) : "-";

            return View(p);
        }

        private string FormatoUsuario(Usuario? u)
        {
            if (u == null) return "-";
            return $"{u.Nombre} {u.Apellido} (#{u.Id})";
        }

        public IActionResult Create(int contratoId)
        {
            ViewBag.Contrato = repoContrato.ObtenerPorId(contratoId);
            return View(new Pago { IdContrato = contratoId, Fecha = DateTime.Today });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Create(Pago p)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Contrato = repoContrato.ObtenerPorId(p.IdContrato);
                return View(p);
            }

            var contrato = repoContrato.ObtenerPorId(p.IdContrato);
            if (contrato == null)
            {
                ModelState.AddModelError("", "El contrato seleccionado no existe.");
                ViewBag.Contrato = null;
                return View(p);
            }

            // Validar que la fecha del pago esté dentro de las fechas del contrato
            if (p.Fecha < contrato.FechaInicio || p.Fecha > contrato.FechaFin)
            {
                ModelState.AddModelError("", $"La fecha del pago debe estar entre {contrato.FechaInicio:dd/MM/yyyy} y {contrato.FechaFin:dd/MM/yyyy}.");
                ViewBag.Contrato = contrato;
                return View(p);
            }

            var userId = HttpContext.Session.GetInt32("UserId") ?? 0;
            repo.Alta(p, userId);
            return RedirectToAction(nameof(Index), new { contratoId = p.IdContrato });
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public IActionResult Anular(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId") ?? 0;
            repo.Anular(id, userId);
            return RedirectToAction(nameof(Index));
        }
    }
}
