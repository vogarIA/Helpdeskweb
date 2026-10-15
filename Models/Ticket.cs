using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HelpDeskWeb.Models
{
    public class Ticket
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El título es obligatorio.")]
        [MaxLength(255)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(255)]
        public string? Description { get; set; }

        public int? TicketStatusId { get; set; }

        [ForeignKey(nameof(TicketStatusId))]
        public TicketStatus? TicketStatus { get; set; }

        public int? TicketPriorityId { get; set; }

        [ForeignKey(nameof(TicketPriorityId))]
        public TicketPriority? TicketPriority { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? ResolvedAt { get; set; }
        public DateTime? ClosedAt { get; set; }
        public DateTime? DueDate { get; set; }

        // Autor y relaciones a usuarios - todas opcionales
        public string? CreatedById { get; set; }
        public string? AssignedToId { get; set; }
        public string? ResolvedById { get; set; }
        public string? ClosedById { get; set; }

        [ForeignKey(nameof(CreatedById))]
        public Usser? CreatedBy { get; set; }

        [ForeignKey(nameof(AssignedToId))]
        public Usser? AssignedTo { get; set; }

        [ForeignKey(nameof(ResolvedById))]
        public Usser? ResolvedBy { get; set; }

        [ForeignKey(nameof(ClosedById))]
        public Usser? ClosedBy { get; set; }
    }
}