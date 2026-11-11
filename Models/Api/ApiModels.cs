using System.ComponentModel.DataAnnotations;

namespace HelpDeskWeb.Models.Api
{
    public class LoginRequest
    {
        [Required]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }

    public class LoginResponse
    {
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public string UserName { get; set; } = string.Empty;
        public IList<string> Roles { get; set; } = new List<string>();
    }

    public class CreateTicketRequest
    {
        [Required(ErrorMessage = "El título es obligatorio.")]
        [MaxLength(255)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(255)]
        public string? Description { get; set; }

        public int? TicketPriorityId { get; set; }

        public DateTime? DueDate { get; set; }
    }

    public class TicketDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Status { get; set; }
        public string? Priority { get; set; }
        public string? CreatedBy { get; set; }
        public string? AssignedTo { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? DueDate { get; set; }
    }
}