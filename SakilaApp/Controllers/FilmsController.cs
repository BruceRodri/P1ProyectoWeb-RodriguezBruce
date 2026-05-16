using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using SakilaApp.Models;

namespace SakilaApp.Controllers
{
    [Authorize]
    public class FilmsController : Controller
    {
        private readonly SakilaContext _context;
        public FilmsController(SakilaContext context) => _context = context;
        //PAGINACION DE 15 PELICULAS
        public async Task<IActionResult> Index(int? page)
        {
            int pageSize = 15;
            // PÁGINA POR DEFECTO LA 1
            int pageNumber = page ?? 1;
            // APLICAMOS EL FILTRO DE ELIMINACIÓN LÓGICA
            var activeFilms = _context.Films
                .Where(f => f.Active)
                .OrderBy(f => f.Title);
            // CALCULAMOS CUÁNTAS PELÍCULAS SALTAR BASÁNDONOS SOLO EN LAS ACTIVAS
            var filmsPage = await activeFilms
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            ViewBag.CurrentPage = pageNumber;
            // CONTEO TOTAL
            ViewBag.TotalPages = (int)Math.Ceiling((double)await activeFilms.CountAsync() / pageSize);
            return View(filmsPage);
        }
        public async Task<IActionResult> Details(int id)
        {
            var film = await _context.Films
                .Include(f => f.FilmActors)
                .ThenInclude(fa => fa.Actor)
                .FirstOrDefaultAsync(f => f.FilmId == id);
            if (film == null) return NotFound();
            return View(film);
        }
        [HttpGet]
        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Film film)
        {
            if (film.LanguageId == 0) film.LanguageId = 1;
            if (!ModelState.IsValid) return View(film);
            film.LastUpdate = DateTime.Now;
            _context.Films.Add(film);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Pelicula creada exitosamente";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var film = await _context.Films.FindAsync(id);
            if (film == null) return NotFound();
            return View(film);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Film film)
        {
            if (id != film.FilmId) return BadRequest();
            if (!ModelState.IsValid) return View(film);
            film.LastUpdate = DateTime.Now;
            _context.Films.Update(film);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Película actualizada";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var film = await _context.Films.FindAsync(id);
            if (film == null) return NotFound();
            return View(film);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var film = await _context.Films.FindAsync(id);
            if (film != null)
            {
                film.Active = false; // DESACTIVACIÓN
                film.LastUpdate = DateTime.Now;
                _context.Update(film);
                await _context.SaveChangesAsync();
                TempData["Success"] = "PELÍCULA DESACTIVADA DEL CATÁLOGO";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
