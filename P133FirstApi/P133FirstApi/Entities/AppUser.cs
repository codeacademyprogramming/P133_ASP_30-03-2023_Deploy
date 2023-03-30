using Microsoft.AspNetCore.Identity;

namespace P133FirstApi.Entities
{
    public class AppUser : IdentityUser
    {
        public string Name { get; set; }
    }
}
