using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SakilaApp.Models;
namespace SakilaApp.Controllers;

[Authorize]
public class CategoriesController : Controller
{
    private readonly SakilaContext _context;
    public CategoriesController(SakilaContext context) => _context = context;
    public async Task<IActionResult> Index()
    {
        // TRAEMOS SOLO LAS CATEGORÍAS QUE ESTÁN "ACTIVAS"
        var activeCategories = await _context.Categories
            .Where(c => c.Active)
            .OrderBy(c => c.Name)
            .ToListAsync();

        return View(activeCategories);
    }
    public async Task<IActionResult> Details(int id)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category == null) return NotFound();
        return View(category);
    }
    [HttpGet]
    public IActionResult Create() => View();
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Category category)
    {
        if (!ModelState.IsValid) return View(category);
        category.CategoryId = 0;
        category.LastUpdate = DateTime.Now;
        _context.Categories.Add(category);
        await _context.SaveChangesAsync();
        TempData["Success"] = "Categoría creada";
        return RedirectToAction(nameof(Index));
    }
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category == null) return NotFound();
        return View(category);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Category category)
    {
        if (id != category.CategoryId) return BadRequest();
        if (!ModelState.IsValid) return View(category);
        category.LastUpdate = DateTime.Now;
        _context.Categories.Update(category);
        await _context.SaveChangesAsync();
        TempData["Success"] = "Categoría actualizada";
        return RedirectToAction(nameof(Index));
    }
    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category == null) return NotFound();
        return View(category);
    }
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category != null)
        {
            // CAMBIAMOS EL ESTADO A FALSE
            category.Active = false;
            category.LastUpdate = DateTime.Now;

            _context.Update(category);
            await _context.SaveChangesAsync();
            TempData["Success"] = "LA CATEGORÍA HA SIDO DESACTIVADA";
        }
        return RedirectToAction(nameof(Index));
    }
}