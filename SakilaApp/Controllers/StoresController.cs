using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SakilaApp.Models;
namespace SakilaApp.Controllers;

[Authorize]
public class StoresController : Controller
{
    private readonly SakilaContext _context;
    public StoresController(SakilaContext context) => _context = context;
    public async Task<IActionResult> Index()
    {
        // SOLO MOSTRAMOS LAS TIENDAS QUE ESTÁN OPERATIVAS (ACTIVAS)
        var activeStores = await _context.Stores
            .Where(s => s.Active)
            .ToListAsync();

        return View(activeStores);
    }
    public async Task<IActionResult> Details(int id)
    {
        var store = await _context.Stores.FindAsync(id);
        if (store == null) return NotFound();
        return View(store);
    }
    [HttpGet]
    public IActionResult Create() => View();
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Store store)
    {
        if (!ModelState.IsValid) return View(store);
        store.LastUpdate = DateTime.Now;
        _context.Stores.Add(store);
        await _context.SaveChangesAsync();
        TempData["Success"] = "Tienda creada";
        return RedirectToAction(nameof(Index));
    }
    [HttpGet]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        // BUSCAMOS LA TIENDA POR ID
        var store = await _context.Stores.FindAsync(id);
        if (store == null) return NotFound();

        // PARA MOSTRAR "ID", "MANAGER STAFF ID" Y "ADDRESS ID"
        // CARGAMOS LAS LISTAS USANDO LOS DATOS REALES DE LA BD
        ViewBag.ManagerStaffId = new SelectList(_context.Stores, "ManagerStaffId", "ManagerStaffId", store.ManagerStaffId);
        ViewBag.AddressId = new SelectList(_context.Stores, "AddressId", "AddressId", store.AddressId);

        return View(store);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Store store)
    {
        if (id != store.StoreId) return BadRequest();
        if (!ModelState.IsValid)
        {
            try
            {
            store.LastUpdate = DateTime.Now;
            _context.Stores.Update(store);
            await _context.SaveChangesAsync();
            TempData["Success"] = "TIENDA ACTUALIZADA CORRECTAMENTE";
            return RedirectToAction(nameof(Index));
        }
            catch (DbUpdateException ex)
            {
                // SI OCURRE EL ERROR DE LLAVE DUPLICADA
                ModelState.AddModelError("AddressId", "ESTA DIRECCIÓN YA ESTÁ ASIGNADA A OTRA TIENDA. POR FAVOR, ELIJA UNA DIFERENTE.");
        }
        }

        // RECARGAMOS LAS LISTAS PARA QUE EL USUARIO PUEDA CORREGIR EL ERROR
        ViewBag.ManagerStaffId = new SelectList(_context.Stores, "ManagerStaffId", "ManagerStaffId", store.ManagerStaffId);
    ViewBag.AddressId = new SelectList(_context.Stores, "AddressId", "AddressId", store.AddressId);
    
    return View(store);
    }
    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var store = await _context.Stores.FindAsync(id);
        if (store == null) return NotFound();
        return View(store);
    }
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var store = await _context.Stores.FindAsync(id);
        if (store != null)
        {
            // MARCAMOS LA TIENDA COMO INACTIVA
            store.Active = false;
            store.LastUpdate = DateTime.Now;

            _context.Update(store);
            await _context.SaveChangesAsync();
            TempData["Success"] = "LA TIENDA HA SIDO DESACTIVADA DEL SISTEMA";
        }
        return RedirectToAction(nameof(Index));
    }
}