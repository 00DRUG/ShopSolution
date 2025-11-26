using FluentValidation;
using Shop.Application.DTOs;

namespace Shop.Application.Validators
{
    public class CreateProductValidator : AbstractValidator<CreateProductDto>
    {
        public CreateProductValidator() 
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Product name is required.").MaximumLength(100);
            //Handing the absolte URL validation **
            RuleFor(x => x.ImgUrl).NotEmpty().WithMessage("Image Url is required.").Must(uri=>Uri.TryCreate(uri, UriKind.Absolute, out _)).WithMessage("Image Url must be a valid URL.");
        }
    }
}
