using ERPLite.Services.DTOs.Inventory;
using FluentValidation;

namespace ERPLite.Services.Validators.Inventory
{
    public class UpdateProductValidator : AbstractValidator<UpdateProductDto>
    {
        public UpdateProductValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Product id is invalid.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Product name is required.")
                .Length(2, 150).WithMessage("Product name must be between 2 and 150 characters.");

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("Price must be greater than zero.");

            RuleFor(x => x.QuantityInStock)
                .GreaterThanOrEqualTo(0).WithMessage("Quantity in stock cannot be negative.");

            RuleFor(x => x.MinStockLevel)
                .GreaterThanOrEqualTo(0).WithMessage("Minimum stock level cannot be negative.");

            RuleFor(x => x.CategoryId)
                .GreaterThan(0).WithMessage("Category is required.");

            RuleFor(x => x.SupplierId)
                .GreaterThan(0).WithMessage("Supplier is required.");
        }
    }
}
