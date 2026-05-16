using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SakilaApp.Models;
namespace SakilaApp.Controllers;

[Authorize]
public class ActorsController : Controller
{
    private readonly SakilaContext _context;
    public ActorsController(SakilaContext context) => _context = context;
    public async Task<IActionResult> Index(int? page)
    {
        int pageSize = 15;
        int pageNumber = page ?? 1;

        // FILTRAMOS SOLO LOS ACTORES QUE ESTÉN ACTIVOS
        var activeActors = _context.Actors
            .Where(a => a.Active)
            .OrderBy(a => a.FirstName);

        var items = await activeActors
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        ViewBag.CurrentPage = pageNumber;
        ViewBag.TotalPages = (int)Math.Ceiling((double)await activeActors.CountAsync() / pageSize);

        return View(items);
    }
    public async Task<IActionResult> Details(int id)
    {
        var actor = await _context.Actors
        .Include(a => a.FilmActors)
        .ThenInclude(fa => fa.Film)
        .FirstOrDefaultAsync(a => a.ActorId == id);
        if (actor == null) return NotFound();
        return View(actor);
    }
    [HttpGet]
    public IActionResult Create() => View();
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Actor actor)
    {
        if (!ModelState.IsValid) return View(actor);
        actor.LastUpdate = DateTime.Now;
        _context.Actors.Add(actor);
        await _context.SaveChangesAsync();
        TempData["Success"] = "Actor creado";
        return RedirectToAction(nameof(Index));
    }
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var actor = await _context.Actors.FindAsync(id);
        if (actor == null) return NotFound();
        return View(actor);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Actor actor)
    {
        if (id != actor.ActorId) return BadRequest();
        if (!ModelState.IsValid) return View(actor);
        actor.LastUpdate = DateTime.Now;
        _context.Actors.Update(actor);
        await _context.SaveChangesAsync();
        TempData["Success"] = "Actor actualizado";
        return RedirectToAction(nameof(Index));
    }
    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var actor = await _context.Actors.FindAsync(id);
        if (actor == null) return NotFound();
        return View(actor);
    }
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var actor = await _context.Actors.FindAsync(id);
        if (actor != null)
        {
            // NO BORRAMOS, SOLO CAMBIAMOS EL "INTERRUPTOR"
            actor.Active = false;
            actor.LastUpdate = DateTime.Now;

            _context.Update(actor);
            await _context.SaveChangesAsync();
            TempData["Success"] = "EL ACTOR HA SIDO DESACTIVADO DEL SISTEMA";
        }
        return RedirectToAction(nameof(Index));
    }
}