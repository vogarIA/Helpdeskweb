using HelpDeskWeb.Data;
using HelpDeskWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Sockets;

namespace HelpDeskWeb.Controllers
{
    [Authorize(Roles = "Admin")]
    public class TicketPriorityController : Controller
    {
        private readonly AppDbContext _appDbContext;

        public TicketPriorityController(AppDbContext dbContext)
        {
            _appDbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            List<TicketPriority> list = await _appDbContext.TicketPriority
                .AsNoTracking()
                .OrderBy(tp => tp.Id)
                .ToListAsync();

            return View(list);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name")] TicketPriority ticketPriority)
        {
            ticketPriority.Name = ticketPriority.Name?.Trim() ?? string.Empty;

            if (await _appDbContext.TicketPriority.AnyAsync(tp => tp.Name == ticketPriority.Name))
                ModelState.AddModelError(nameof(ticketPriority.Name), "Ya existe una prioridad con ese nombre.");

            if (!ModelState.IsValid)
                return View(ticketPriority);

            // CreatedAt y UpdatedAt los asigna SQL Server (GETUTCDATE) al insertar
            _appDbContext.TicketPriority.Add(ticketPriority);
            await _appDbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var ticketPriority = await _appDbContext.TicketPriority.FindAsync(id);
            if (ticketPriority == null) return NotFound();

            return View(ticketPriority);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] TicketPriority ticketPriority)
        {
            if (id != ticketPriority.Id) return NotFound();

            ticketPriority.Name = ticketPriority.Name?.Trim() ?? string.Empty;

            if (await _appDbContext.TicketPriority.AnyAsync(tp => tp.Name == ticketPriority.Name && tp.Id != id))
                ModelState.AddModelError(nameof(ticketPriority.Name), "Ya existe una prioridad con ese nombre.");

            if (!ModelState.IsValid)
                return View(ticketPriority);

            var existente = await _appDbContext.TicketPriority.FindAsync(id);
            if (existente == null) return NotFound();

            existente.Name = ticketPriority.Name;
            // El GETUTCDATE() del SQL solo aplica al insertar, en las ediciones se asigna aquí
            existente.UpdatedAt = DateTime.UtcNow;

            await _appDbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var ticketPriority = await _appDbContext.TicketPriority
                .AsNoTracking()
                .FirstOrDefaultAsync(tp => tp.Id == id);
            if (ticketPriority == null) return NotFound();

            return View(ticketPriority);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ticketPriority = await _appDbContext.TicketPriority.FindAsync(id);
            if (ticketPriority == null) return NotFound();

            _appDbContext.TicketPriority.Remove(ticketPriority);
            await _appDbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // ===== Asignar prioridad a los tickets =====

        [HttpGet]
        public async Task<IActionResult> Asignar()
        {
            List<Ticket> tickets = await _appDbContext.Ticket
                .AsNoTracking()
                .Include(t => t.TicketPriority)
                .OrderBy(t => t.Id)
                .ToListAsync();

            ViewBag.Priorities = await _appDbContext.TicketPriority
                .AsNoTracking()
                .OrderBy(tp => tp.Id)
                .ToListAsync();

            return View(tickets);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AsignarPrioridad(int id, int? priorityId)
        {
            var ticket = await _appDbContext.Ticket.FindAsync(id);
            if (ticket == null) return NotFound();

            // null = quitar la prioridad; si viene un id, debe existir
            if (priorityId != null && !await _appDbContext.TicketPriority.AnyAsync(tp => tp.Id == priorityId))
                return BadRequest();

            ticket.TicketPriorityId = priorityId;
            // El GETUTCDATE() del SQL solo aplica al insertar, en las ediciones se asigna aquí
            ticket.UpdatedAt = DateTime.UtcNow;

            await _appDbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Asignar));
        }
    }
}