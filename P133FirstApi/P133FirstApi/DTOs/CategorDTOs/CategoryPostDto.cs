using FluentValidation;
using P133FirstApi.DataAccessLayer;

namespace P133FirstApi.DTOs.CategorDTOs
{
    /// <summary>
    /// Category Create Object
    /// </summary>
    public class CategoryPostDto
    {
        /// <summary>
        /// Category Name
        /// </summary>
        public string Name { get; set; }
        public string Test { get; set; }
    }

    public class CategoryPostDtoValidator : AbstractValidator<CategoryPostDto>
    {
        public CategoryPostDtoValidator(AppDbContext context)
        {
            RuleFor(r => r.Name)
                .MaximumLength(100).WithMessage("Maksimum 100 Simvol Ola Bile")
                .MinimumLength(10).WithMessage("Minimum 10 Simvol Ola Biler")
                .NotEmpty().WithMessage("Mecburdiu");

            RuleFor(r => r).Custom(async (obj, validate) =>
            {
                if (context.Categories.Any(x=>x.Name.ToLower() == obj.Name.Trim().ToLower()))
                {
                    validate.AddFailure("Name","Eyni Add Category Var");
                }
            });
        }
    }
}
