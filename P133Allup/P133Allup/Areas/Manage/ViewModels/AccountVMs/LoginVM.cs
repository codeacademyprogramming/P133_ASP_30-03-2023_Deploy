using System.ComponentModel.DataAnnotations;

namespace P133Allup.Areas.Manage.ViewModels.AccountVMs
{
    public class LoginVM
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public bool RemindMe { get; set; }
    }
}
