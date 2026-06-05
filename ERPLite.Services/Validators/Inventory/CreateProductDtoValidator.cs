using ERPLite.Services.DTOs.Inventory;
using FluentValidation;

namespace ERPLite.Services.Validators.Inventory
{
    public class CreateProductDtoValidator : AbstractValidator<CreateProductDto>
    {
        public CreateProductDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(150);

            RuleFor(x => x.SellingPrice)
                .GreaterThan(0);

            RuleFor(x => x.CostPrice)
             .GreaterThan(0);

            RuleFor(x => x.QuantityInStock)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.MinStockLevel)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.CategoryId)
                .GreaterThan(0);

            RuleFor(x => x.SupplierId)
                .GreaterThan(0);
        }
    }
}
