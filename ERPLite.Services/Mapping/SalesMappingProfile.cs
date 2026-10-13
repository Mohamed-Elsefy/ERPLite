using AutoMapper;
using ERPLite.Data.Entities.Sales;
using ERPLite.Services.DTOs.Sales;

namespace ERPLite.Services.Mapping
{
    public class SalesMappingProfile : Profile
    {
        public SalesMappingProfile()
        {
            CreateMap<Customer, CustomerDto>();
            CreateMap<CreateCustomerDto, Customer>();
            CreateMap<UpdateCustomerDto, Customer>();

            CreateMap<OrderItem, OrderItemDto>()
                .ForMember(d => d.ProductName, o => o.MapFrom(s => s.Product.Name));

            CreateMap<Order, OrderDto>()
                .ForMember(d => d.CustomerName, o => o.MapFrom(s => s.Customer.FullName))
                .ForMember(d => d.Items, o => o.MapFrom(s => s.OrderItems))
                .ForMember(d => d.DepartmentId, o => o.MapFrom(s =>
                    s.CreatedByUser != null && s.CreatedByUser.Employee != null
                        ? s.CreatedByUser.Employee.DepartmentId
                        : 0));

            CreateMap<Payment, PaymentDto>();
            CreateMap<CreatePaymentDto, Payment>();
        }
    }
}