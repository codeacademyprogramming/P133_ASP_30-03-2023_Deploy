using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace P133SignalR.Models
{
    public class AppUser : IdentityUser
    {
        [StringLength(100)]
        public string? ConnectionId { get; set; }
    }
}
