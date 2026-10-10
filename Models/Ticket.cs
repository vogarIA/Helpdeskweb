using System.ComponentModel.DataAnnotations;

namespace HelpDeskWeb.Models
{
    public class Ticket
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El título es obligatorio.")]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(2000)]
        public string? Description { get; set; }

        public int? TicketPriorityId { get; set; }
        public TicketPriority? TicketPriority { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}