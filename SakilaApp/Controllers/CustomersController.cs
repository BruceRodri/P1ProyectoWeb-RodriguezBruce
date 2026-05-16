using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SakilaApp.Models;
namespace SakilaApp.Controllers;

[Authorize]
public class CustomersController : Controller
{
    private readonly SakilaContext _context;
    public CustomersController(SakilaContext context) => _context =
   context;
    public async Task<IActionResult> Index(int? page)
    {
        int pageSize = 15;
        int pageNumber = page ?? 1;

        // APLICAMOS EL FILTRO: SOLO TRAEMOS LOS QUE NO HAN SIDO "ELIMINADOS"
        var source = _context.Customers
            .Where(c => c.Active == true)
            .OrderBy(c => c.FirstName);

        var items = await source
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        ViewBag.CurrentPage = pageNumber;
        ViewBag.TotalPages = (int)Math.Ceiling((double)await source.CountAsync() / pageSize);
        return View(items);
    }
    public async Task<IActionResult> Details(int id)
    {
        var customer = await _context.Customers.FindAsync(id);
        if (customer == null) return NotFound();
        return View(customer);
    }
    [HttpGet]
    public IActionResult Create()
    {
        ViewBag.StoreId = new SelectList(_context.Stores, "StoreId", "StoreId");
        ViewBag.AddressId = new SelectList(_context.Stores, "AddressId", "AddressId");

        return View();
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Customer customer)
    {
        if (ModelState.IsValid)
        {
            customer.CreateDate = DateTime.Now;
            customer.LastUpdate = DateTime.Now;
            _context.Add(customer);
            await _context.SaveChangesAsync();
            TempData["Success"] = "CLIENTE CREADO CON ÉXITO";
            return RedirectToAction(nameof(Index));
        }
        // SI HAY ERROR, VOLVEMOS A CARGAR LAS LISTAS PARA NO PERDER EL DISEÑO
        ViewBag.StoreId = new SelectList(_context.Stores, "StoreId", "StoreId", customer.StoreId);
        ViewBag.AddressId = new SelectList(_context.Stores, "AddressId", "AddressId", customer.AddressId);

        return View(customer);
    }
    [HttpGet]
    // GET: Customers/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var customer = await _context.Customers.FindAsync(id);
        if (customer == null) return NotFound();

        // ESTO ES LO QUE FALTA: CARGAR LOS DATOS PARA LOS SELECTS
        // SelectList(Origen de datos, Valor que se guarda, Texto que se muestra, Valor seleccionado)
        ViewBag.StoreId = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_context.Stores, "StoreId", "StoreId", customer.StoreId);

        // EN SAKILA, PODEMOS USAR LA TABLA STORES PARA SACAR IDS DE DIRECCIÓN RÁPIDAMENTE
        ViewBag.AddressId = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_context.Stores, "AddressId", "AddressId", customer.AddressId);

        return View(customer);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Customer customer)
    {
        if (id != customer.CustomerId) return BadRequest();

        if (ModelState.IsValid)
        {
            try
            {
                customer.LastUpdate = DateTime.Now;
                _context.Update(customer);
                await _context.SaveChangesAsync();
                TempData["Success"] = "CLIENTE ACTUALIZADO CORRECTAMENTE";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                // SI SALE EL ERROR DE FOREIGN KEY AQUÍ, ES PORQUE EL ID SELECCIONADO NO EXISTE
                ModelState.AddModelError("", "ERROR DE INTEGRIDAD: ASEGÚRATE DE SELECCIONAR UNA TIENDA Y DIRECCIÓN VÁLIDAS.");
            }
        }

        // SI LLEGAMOS AQUÍ ES PORQUE ALGO FALLÓ, RECARGAMOS LAS LISTAS
        ViewBag.StoreId = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_context.Stores, "StoreId", "StoreId", customer.StoreId);
        ViewBag.AddressId = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_context.Stores, "AddressId", "AddressId", customer.AddressId);

        return View(customer);
    }
    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var customer = await _context.Customers.FindAsync(id);
        if (customer == null) return NotFound();
        return View(customer);
    }
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var customer = await _context.Customers.FindAsync(id);

        if (customer != null)
        {
            // EN LUGAR DE BORRAR, CAMBIAMOS EL ESTADO A FALSE
            customer.Active = false;
            customer.LastUpdate = DateTime.Now;

            _context.Update(customer); // ESTO AHORA ES UN UPDATE, NO UN DELETE
            await _context.SaveChangesAsync();

            TempData["Success"] = "EL CLIENTE HA SIDO DESACTIVADO CORRECTAMENTE";
        }

        return RedirectToAction(nameof(Index));
    }
}