using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SakilaApp.Models;
namespace SakilaApp.Controllers;

[Authorize]
public class InventoriesController : Controller
{
    private readonly SakilaContext _context;
    public InventoriesController(SakilaContext context) => _context =
   context;
    public async Task<IActionResult> Index(int? page)
    {
        int pageSize = 15;
        int pageNumber = page ?? 1;

        // FILTRAMOS SOLO LOS ACTIVOS E INCLUIMOS LA PELÍCULA PARA VER EL TÍTULO
        var activeInventory = _context.Inventories
            .Include(i => i.Film)
            .Where(i => i.Active)
            .OrderBy(i => i.InventoryId);

        var items = await activeInventory
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        ViewBag.CurrentPage = pageNumber;
        ViewBag.TotalPages = (int)Math.Ceiling((double)await activeInventory.CountAsync() / pageSize);

        return View(items);
    }

    public async Task<IActionResult> Details(int? id) // CAMBIA A int? POR SEGURIDAD
    {
        if (id == null) return NotFound();

        // USAMOS .INCLUDE PARA CARGAR LA RELACIÓN CON LA PELÍCULA
        var inventory = await _context.Inventories
            .Include(i => i.Film)
            .FirstOrDefaultAsync(m => m.InventoryId == id);

        if (inventory == null) return NotFound();

        return View(inventory);
    }
    [HttpGet]
    public IActionResult Create()
    {
        // CARGAMOS LAS PELÍCULAS Y TIENDAS DISPONIBLES EN SAKILA
        ViewBag.FilmId = new SelectList(_context.Films, "FilmId", "Title");
        ViewBag.StoreId = new SelectList(_context.Stores, "StoreId", "StoreId");
        return View();
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Inventory inventory)
    {
        if (ModelState.IsValid)
        {
            inventory.LastUpdate = DateTime.Now;
            _context.Inventories.Add(inventory);
            await _context.SaveChangesAsync();
            TempData["Success"] = "INVENTARIO CREADO EXITOSAMENTE";
            return RedirectToAction(nameof(Index));
        }

        // SI ALGO FALLA, RECARGAMOS LAS LISTAS PARA NO ROMPER EL DISEÑO BOOTSTRAP
        ViewBag.FilmId = new SelectList(_context.Films, "FilmId", "Title", inventory.FilmId);
        ViewBag.StoreId = new SelectList(_context.Stores, "StoreId", "StoreId", inventory.StoreId);
        return View(inventory);
    }
    [HttpGet]
    // GET: Inventories/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var inventory = await _context.Inventories.FindAsync(id);
        if (inventory == null) return NotFound();

        // CARGAMOS LAS LISTAS PARA LOS DESPLEGABLES (SELECTS)
        ViewBag.FilmId = new SelectList(_context.Films.OrderBy(f => f.Title), "FilmId", "Title", inventory.FilmId);
        ViewBag.StoreId = new SelectList(_context.Stores, "StoreId", "StoreId", inventory.StoreId);

        return View(inventory);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Inventory inventory)
    {
        if (id != inventory.InventoryId) return BadRequest();

        if (ModelState.IsValid)
        {
            try
            {
                inventory.LastUpdate = DateTime.Now;
                _context.Update(inventory);
                await _context.SaveChangesAsync();
                TempData["Success"] = "INVENTARIO ACTUALIZADO CORRECTAMENTE";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "ERROR DE INTEGRIDAD: ASEGÚRESE DE QUE LA TIENDA Y LA PELÍCULA SEAN VÁLIDAS.");
            }
        }

        // SI HAY ERROR, RECARGAMOS LAS LISTAS PARA NO ROMPER LA VISTA
        ViewBag.FilmId = new SelectList(_context.Films.OrderBy(f => f.Title), "FilmId", "Title", inventory.FilmId);
        ViewBag.StoreId = new SelectList(_context.Stores, "StoreId", "StoreId", inventory.StoreId);
        return View(inventory);
    }
    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var inventory = await _context.Inventories.FindAsync(id);
        if (inventory == null) return NotFound();
        return View(inventory);
    }
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var inventory = await _context.Inventories.FindAsync(id);
        if (inventory != null)
        {
            // EN LUGAR DE REMOVE, HACEMOS UPDATE DEL ESTADO
            inventory.Active = false;
            inventory.LastUpdate = DateTime.Now;

            _context.Update(inventory);
            await _context.SaveChangesAsync();
            TempData["Success"] = "EL ARTÍCULO HA SIDO RETIRADO DEL INVENTARIO ACTIVO";
        }
        return RedirectToAction(nameof(Index));
    }
}