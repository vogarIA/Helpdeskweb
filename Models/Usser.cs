using Microsoft.AspNetCore.Identity;

namespace HelpDeskWeb.Models
{
    public class Usser : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}
