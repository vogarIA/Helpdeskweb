using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HelpDeskWeb.Models;
using HelpDeskWeb.Data;

namespace HelpDeskWeb.Controllers
{
    public class TicketStatusController : Controller
    {
        private AppDbContext _appDbContext;

        public TicketStatusController(AppDbContext dbContext)
        {
            _appDbContext = dbContext;
        }

        [HttpGet]
        public IActionResult Index()
        {
            List<TicketStatus> ticketStatusList = _appDbContext.TicketStatus.ToList();
            return View(ticketStatusList);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(TicketStatus ticketStatus)
        {
            if (ModelState.IsValid)
            {
                _appDbContext.TicketStatus.Add(ticketStatus);
                await _appDbContext.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(ticketStatus);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var ticketStatus = await _appDbContext.TicketStatus.FindAsync(id);
            if (ticketStatus == null) return NotFound();

            return View(ticketStatus);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TicketStatus ticketStatus)
        {
            if (id != ticketStatus.Id) return NotFound();

            // Name tiene índice único, así que se revisa antes de guardar
            if (_appDbContext.TicketStatus.Any(ts => ts.Name == ticketStatus.Name && ts.Id != id))
                ModelState.AddModelError(nameof(ticketStatus.Name), "Ya existe un estado con ese nombre.");

            if (!ModelState.IsValid) return View(ticketStatus);

            var existente = await _appDbContext.TicketStatus.FindAsync(id);
            if (existente == null) return NotFound();

            existente.Name = ticketStatus.Name;
            // El GETUTCDATE() del SQL solo aplica al insertar, en las ediciones se asigna aquí
            existente.UpdatedAt = DateTime.UtcNow;

            await _appDbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var ticketStatus = await _appDbContext.TicketStatus.FindAsync(id);
            if (ticketStatus == null) return NotFound();

            return View(ticketStatus);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ticketStatus = await _appDbContext.TicketStatus.FindAsync(id);
            if (ticketStatus == null) return NotFound();

            _appDbContext.TicketStatus.Remove(ticketStatus);
            await _appDbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}