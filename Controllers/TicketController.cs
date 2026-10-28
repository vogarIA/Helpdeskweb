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

        private async Task CargarPrioridades()
        {
            var priorities = await _appDbContext.TicketPriority
                .AsNoTracking()
                .OrderBy(tp => tp.Id)
                .ToListAsync();

            ViewBag.Priorities = new SelectList(priorities, "Id", "Name");
        }
    }
}