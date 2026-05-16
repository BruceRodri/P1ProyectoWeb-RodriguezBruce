using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SakilaApp.Models;

namespace SakilaApp.Controllers;

public class HomeController : Controller
{
    private readonly SakilaContext _context;

    // INYECTAMOS EL CONTEXTO PARA PODER CONSULTAR LA BASE DE DATOS
    public HomeController(SakilaContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        // 1. OBTENEMOS LOS CONTEOS FILTRANDO POR LOS QUE ESTÁN ACTIVOS (ELIMINACIÓN LÓGICA)
        ViewBag.TotalFilms = await _context.Films.CountAsync(f => f.Active);
        ViewBag.TotalCustomers = await _context.Customers.CountAsync(c => c.Active);
        ViewBag.TotalRentals = await _context.Rentals.CountAsync(r => r.Active);
        ViewBag.TotalStores = await _context.Stores.CountAsync(s => s.Active);

        // 2. TRAEMOS LOS 5 ALQUILERES MÁS RECIENTES PARA MOSTRARLOS EN UNA MINI-TABLA
        var recentRentals = await _context.Rentals
            .Where(r => r.Active)
            .OrderByDescending(r => r.RentalDate)
            .Take(5)
            .ToListAsync();

        // 3. PASAMOS LA LISTA DE RENTAS A LA VISTA
        return View(recentRentals);
    }

    public IActionResult Privacy()
    {
        return View();
    }
}