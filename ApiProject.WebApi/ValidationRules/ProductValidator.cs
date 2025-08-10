using ApiProject.WebApi.Entities;
using FluentValidation;

namespace ApiProject.WebApi.ValidationRules
{
    public class ProductValidator:AbstractValidator<Product>
    {
        public ProductValidator()
        {
            RuleFor(x => x.ProductName).NotEmpty().WithMessage("Ürün adı boş olamaz.");
            RuleFor(x => x.ProductName).MinimumLength(2).WithMessage("En az 2 karakter girilmesi gerekir.");
            RuleFor(x => x.ProductName).MaximumLength(50).WithMessage("En fazla 40 karakter girilmesi gerekir.");

            RuleFor(x => x.Price).GreaterThanOrEqualTo(0).WithMessage("Ürün fiyatı negatif değer olamaz");

            RuleFor(x => x.ProductDescription).NotEmpty().WithMessage("Ürün açıklaması boş geçilemez.");
        }

    }
}
