using System.Security.Claims;
using HelpDeskWeb.Data;
using HelpDeskWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace HelpDeskWeb.Controllers
{
    [Authorize] // cualquier usuario con sesión iniciada, no solo Admin
    public class TicketController : Controller
    {
        private readonly AppDbContext _appDbContext;

        public TicketController(AppDbContext dbContext)
        {
            _appDbContext = dbContext;
        }

        // Admin ve todos los tickets, un usuario normal solo los suyos
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var query = _appDbContext.Ticket
                .AsNoTracking()
                .Include(t => t.TicketStatus)
                .Include(t => t.TicketPriority)
                .Include(t => t.CreatedBy)
                .AsQueryable();

            if (!User.IsInRole("Admin"))
                query = query.Where(t => t.CreatedById == userId);

            List<Ticket> tickets = await query
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();

            return View(tickets);
        }

        [Authorize(Roles = "Admin,User")]
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var ticket = await _appDbContext.Ticket
                .AsNoTracking()
                .Include(t => t.TicketStatus)
                .Include(t => t.TicketPriority)
                .Include(t => t.CreatedBy)
                .Include(t => t.AssignedTo)
                .Include(t => t.ResolvedBy)
                .Include(t => t.ClosedBy)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (ticket == null) return NotFound();

            // Un usuario normal solo puede ver sus propios tickets
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!User.IsInRole("Admin") && ticket.CreatedById != userId)
                return Forbid();

            return View(ticket);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await CargarPrioridades();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Title,Description,TicketPriorityId,DueDate")] Ticket ticket)
        {
            if (ticket.TicketPriorityId != null &&
                !await _appDbContext.TicketPriority.AnyAsync(tp => tp.Id == ticket.TicketPriorityId))
            {
                ModelState.AddModelError(nameof(ticket.TicketPriorityId), "La prioridad no es válida.");
            }

            if (!ModelState.IsValid)
            {
                await CargarPrioridades();
                return View(ticket);
            }

            // Estos campos NO vienen del formulario, los pone el servidor
            ticket.CreatedById = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ticket.TicketStatusId = await _appDbContext.TicketStatus
                .OrderBy(ts => ts.Id)
                .Select(ts => (int?)ts.Id)
                .FirstOrDefaultAsync(); // estado inicial = el primero del catálogo

            _appDbContext.Ticket.Add(ticket);
            await _appDbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var ticket = await _appDbContext.Ticket.FindAsync(id);
            if (ticket == null) return NotFound();

            await CargarCatalogos();
            return View(ticket);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,
            [Bind("Id,Title,Description,TicketStatusId,TicketPriorityId,AssignedToId,DueDate")] Ticket ticket)
        {
            if (id != ticket.Id) return NotFound();

            if (ticket.TicketStatusId != null &&
                !await _appDbContext.TicketStatus.AnyAsync(ts => ts.Id == ticket.TicketStatusId))
                ModelState.AddModelError(nameof(ticket.TicketStatusId), "El estado no es válido.");

            if (ticket.TicketPriorityId != null &&
                !await _appDbContext.TicketPriority.AnyAsync(tp => tp.Id == ticket.TicketPriorityId))
                ModelState.AddModelError(nameof(ticket.TicketPriorityId), "La prioridad no es válida.");

            if (ticket.AssignedToId != null &&
                !await _appDbContext.Users.AnyAsync(u => u.Id == ticket.AssignedToId))
                ModelState.AddModelError(nameof(ticket.AssignedToId), "El usuario no es válido.");

            if (!ModelState.IsValid)
            {
                await CargarCatalogos();
                return View(ticket);
            }

            var existente = await _appDbContext.Ticket.FindAsync(id);
            if (existente == null) return NotFound();

            existente.Title = ticket.Title;
            existente.Description = ticket.Description;
            existente.TicketStatusId = ticket.TicketStatusId;
            existente.TicketPriorityId = ticket.TicketPriorityId;
            existente.AssignedToId = ticket.AssignedToId;
            existente.DueDate = ticket.DueDate;
            // El GETUTCDATE() del SQL solo aplica al insertar, en las ediciones se asigna aquí
            existente.UpdatedAt = DateTime.UtcNow;

            await _appDbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Details), new { id });
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var ticket = await _appDbContext.Ticket
                .AsNoTracking()
                .Include(t => t.TicketStatus)
                .Include(t => t.TicketPriority)
                .Include(t => t.CreatedBy)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (ticket == null) return NotFound();

            return View(ticket);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ticket = await _appDbContext.Ticket.FindAsync(id);
            if (ticket == null) return NotFound();

            _appDbContext.Ticket.Remove(ticket);
            await _appDbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private async Task CargarPrioridades()
        {
            var priorities = await _appDbContext.TicketPriority
                .AsNoTracking()
                .OrderBy(tp => tp.Id)
                .ToListAsync();

            ViewBag.Priorities = new SelectList(priorities, "Id", "Name");
        }

        private async Task CargarCatalogos()
        {
            ViewBag.Statuses = new SelectList(
                await _appDbContext.TicketStatus.AsNoTracking().OrderBy(ts => ts.Id).ToListAsync(),
                "Id", "Name");

            ViewBag.Priorities = new SelectList(
                await _appDbContext.TicketPriority.AsNoTracking().OrderBy(tp => tp.Id).ToListAsync(),
                "Id", "Name");

            ViewBag.Users = new SelectList(
                await _appDbContext.Users.AsNoTracking().OrderBy(u => u.UserName).ToListAsync(),
                "Id", "UserName");
        }
    }
}