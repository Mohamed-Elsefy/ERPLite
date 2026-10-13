using AutoMapper;
using ERPLite.Services.DTOs.Inventory;
using ERPLite.Web.Models.Categories;
using ERPLite.Web.Models.Products;
using ERPLite.Web.Models.Suppliers;

namespace ERPLite.Web.Mapping
{
    public class InventoryWebMappingProfile : Profile
    {
        public InventoryWebMappingProfile()
        {
            // Category ViewModels
            CreateMap<CategoryDto, CategoryDetailsViewModel>()
                .ForMember(dest => dest.AssociatedProducts, opt => opt.MapFrom(src => src.Products));

            // Product ViewModels
            CreateMap<ProductDto, ProductFormViewModel>()
                .ForMember(dest => dest.IsEditMode, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.CategoriesList, opt => opt.Ignore())
                .ForMember(dest => dest.SuppliersList, opt => opt.Ignore());

            // Supplier ViewModels
            CreateMap<SupplierDto, SupplierDetailsViewModel>()
                .ForMember(dest => dest.SuppliedProducts, opt => opt.MapFrom(src => src.Products));
        }
    }
}
