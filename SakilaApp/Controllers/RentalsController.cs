using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SakilaApp.Models;
namespace SakilaApp.Controllers;

[Authorize]
public class RentalsController : Controller
{
    private readonly SakilaContext _context;
    public RentalsController(SakilaContext context) => _context = context;
    public async Task<IActionResult> Index(int? page)
    {
        int pageSize = 15;
        int pageNumber = page ?? 1;

        // SOLO TRAEMOS ALQUILERES ACTIVOS
        var activeRentals = _context.Rentals
            .Where(r => r.Active)
            .OrderByDescending(r => r.RentalDate); // MOSTRAMOS LAS MÁS RECIENTES PRIMERO

        var items = await activeRentals
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        ViewBag.CurrentPage = pageNumber;
        ViewBag.TotalPages = (int)Math.Ceiling((double)await activeRentals.CountAsync() / pageSize);

        return View(items);
    }
    public async Task<IActionResult> Details(int id)
    {
        var rental = await _context.Rentals.FindAsync(id);
        if (rental == null) return NotFound();
        return View(rental);
    }
    [HttpGet]
    public IActionResult Create() => View();
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Rental rental)
    {
        rental.StaffId = 1;
        if (!ModelState.IsValid) return View(rental);
        rental.LastUpdate = DateTime.Now;
        _context.Rentals.Add(rental);
        await _context.SaveChangesAsync();
        TempData["Success"] = "Alquiler creado";
        return RedirectToAction(nameof(Index));
    }
    [HttpGet]
    // GET: Rentals/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();
        var rental = await _context.Rentals.FindAsync(id);
        if (rental == null) return NotFound();

        // CARGAMOS LAS LISTAS PARA QUE EL USUARIO ELIJA OPCIONES REALES
        ViewBag.CustomerId = new SelectList(_context.Customers, "CustomerId", "LastName", rental.CustomerId);
        ViewBag.InventoryId = new SelectList(_context.Inventories, "InventoryId", "InventoryId", rental.InventoryId);
        // IMPORTANTE: CARGAR EL STAFF PARA EVITAR EL ERROR DE SQL
        ViewBag.StaffId = new SelectList(_context.Stores, "ManagerStaffId", "ManagerStaffId", rental.StaffId);

        return View(rental);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Rental rental)
    {
        if (id != rental.RentalId) return BadRequest();

        if (ModelState.IsValid)
        {
            try
            {
                rental.LastUpdate = DateTime.Now;
                _context.Update(rental);
                await _context.SaveChangesAsync();
                TempData["Success"] = "ALQUILER ACTUALIZADO";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "ERROR DE INTEGRIDAD: EL ID DEL EMPLEADO O INVENTARIO NO ES VÁLIDO.");
            }
        }

        // SI FALLA, RECARGAMOS LAS LISTAS
        ViewBag.CustomerId = new SelectList(_context.Customers, "CustomerId", "LastName", rental.CustomerId);
        ViewBag.InventoryId = new SelectList(_context.Inventories, "InventoryId", "InventoryId", rental.InventoryId);
        ViewBag.StaffId = new SelectList(_context.Stores, "ManagerStaffId", "ManagerStaffId", rental.StaffId);
        return View(rental);
    }
    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var rental = await _context.Rentals.FindAsync(id);
        if (rental == null) return NotFound();
        return View(rental);
    }
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var rental = await _context.Rentals.FindAsync(id);
        if (rental != null)
        {
            // EN LUGAR DE BORRAR LA TRANSACCIÓN, LA MARCAMOS COMO INACTIVA (ANULADA)
            rental.Active = false;
            rental.LastUpdate = DateTime.Now;

            _context.Update(rental);
            await _context.SaveChangesAsync();
            TempData["Success"] = "EL ALQUILER HA SIDO ANULADO CORRECTAMENTE";
        }
        return RedirectToAction(nameof(Index));
    }
}