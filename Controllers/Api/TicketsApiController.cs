using System.Security.Claims;
using HelpDeskWeb.Data;
using HelpDeskWeb.Models;
using HelpDeskWeb.Models.Api;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HelpDeskWeb.Controllers.Api
{
    [ApiController]
    [Route("api/tickets")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class TicketsApiController : ControllerBase
    {
        private readonly AppDbContext _appDbContext;

        public TicketsApiController(AppDbContext dbContext)
        {
            _appDbContext = dbContext;
        }

        // GET api/tickets: Admin ve todos, un usuario normal solo los suyos
        [HttpGet]
        public async Task<ActionResult<List<TicketDto>>> GetAll()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var query = _appDbContext.Ticket
                .AsNoTracking()
                .Include(t => t.TicketStatus)
                .Include(t => t.TicketPriority)
                .Include(t => t.CreatedBy)
                .Include(t => t.AssignedTo)
                .AsQueryable();

            if (!User.IsInRole("Admin"))
                query = query.Where(t => t.CreatedById == userId);

            var tickets = await query
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();

            return Ok(tickets.Select(ToDto).ToList());
        }

        // GET api/tickets/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<TicketDto>> GetById(int id)
        {
            var ticket = await _appDbContext.Ticket
                .AsNoTracking()
                .Include(t => t.TicketStatus)
                .Include(t => t.TicketPriority)
                .Include(t => t.CreatedBy)
                .Include(t => t.AssignedTo)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (ticket == null) return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!User.IsInRole("Admin") && ticket.CreatedById != userId)
                return Forbid(JwtBearerDefaults.AuthenticationScheme);

            return Ok(ToDto(ticket));
        }

        // POST api/tickets
        [HttpPost]
        public async Task<ActionResult<TicketDto>> Create([FromBody] CreateTicketRequest request)
        {
            if (request.TicketPriorityId != null &&
                !await _appDbContext.TicketPriority.AnyAsync(tp => tp.Id == request.TicketPriorityId))
            {
                return BadRequest(new { message = "La prioridad no es válida." });
            }

            var ticket = new Ticket
            {
                Title = request.Title.Trim(),
                Description = request.Description,
                TicketPriorityId = request.TicketPriorityId,
                DueDate = request.DueDate,
                // Estos campos los pone el servidor, no el cliente
                CreatedById = User.FindFirstValue(ClaimTypes.NameIdentifier),
                TicketStatusId = await _appDbContext.TicketStatus
                    .OrderBy(ts => ts.Id)
                    .Select(ts => (int?)ts.Id)
                    .FirstOrDefaultAsync()
            };

            _appDbContext.Ticket.Add(ticket);
            await _appDbContext.SaveChangesAsync();

            var creado = await _appDbContext.Ticket
                .AsNoTracking()
                .Include(t => t.TicketStatus)
                .Include(t => t.TicketPriority)
                .Include(t => t.CreatedBy)
                .Include(t => t.AssignedTo)
                .FirstAsync(t => t.Id == ticket.Id);

            return CreatedAtAction(nameof(GetById), new { id = ticket.Id }, ToDto(creado));
        }

        private static TicketDto ToDto(Ticket t) => new TicketDto
        {
            Id = t.Id,
            Title = t.Title,
            Description = t.Description,
            Status = t.TicketStatus?.Name,
            Priority = t.TicketPriority?.Name,
            CreatedBy = t.CreatedBy?.UserName,
            AssignedTo = t.AssignedTo?.UserName,
            CreatedAt = t.CreatedAt,
            DueDate = t.DueDate
        };
    }
}