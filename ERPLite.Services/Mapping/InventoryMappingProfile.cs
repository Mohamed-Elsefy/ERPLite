using AutoMapper;
using ERPLite.Data.Entities.Inventory;
using ERPLite.Services.DTOs.Inventory;

namespace ERPLite.Services.Mapping
{
    public class InventoryMappingProfile : Profile
    {
        public InventoryMappingProfile()
        {
            CreateMap<Category, CategoryDto>();
            CreateMap<CreateCategoryDto, Category>();
            CreateMap<UpdateCategoryDto, Category>();

            CreateMap<Supplier, SupplierDto>();
            CreateMap<CreateSupplierDto, Supplier>();

            CreateMap<Product, ProductDto>()
                .ForMember(
                    d => d.CategoryName,
                    o => o.MapFrom(s => s.Category.Name))
                .ForMember(
                    d => d.SupplierName,
                    o => o.MapFrom(s => s.Supplier.Name)).ReverseMap();

            CreateMap<Product, UpdateProductDto>().ReverseMap();
            CreateMap<CreateProductDto, Product>().ReverseMap();

            CreateMap<UpdateSupplierDto, Supplier>().ReverseMap();
            CreateMap<SupplierDto, Supplier>();


        }
    }
}
