using FluentValidation;

namespace P133FirstApi.DTOs.AuthDTOs
{
    public class RegisterDto
    {
        public string Name { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class RegisterDtoValidator : AbstractValidator<RegisterDto>
    {
        public RegisterDtoValidator()
        {
            RuleFor(r=>r.Email)
                .EmailAddress().WithMessage("Duzgun Email")
                .NotEmpty().WithMessage("Bos Ola Bilmez");
        }
    }
}
