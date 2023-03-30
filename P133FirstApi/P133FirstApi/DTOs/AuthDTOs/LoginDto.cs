using FluentValidation;

namespace P133FirstApi.DTOs.AuthDTOs
{
    public class LoginDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class LoginDtoValidator : AbstractValidator<LoginDto>
    {
        public LoginDtoValidator()
        {
            RuleFor(r => r.Email)
               .EmailAddress().WithMessage("Duzgun Email")
               .NotEmpty().WithMessage("Bos Ola Bilmez");
        }
    }
}
