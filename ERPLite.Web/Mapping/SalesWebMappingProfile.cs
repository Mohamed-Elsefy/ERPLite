using AutoMapper;
using ERPLite.Services.DTOs.Sales;
using ERPLite.Web.Models.Customers;
using ERPLite.Web.Models.Orders;
using ERPLite.Web.Models.Payments;

namespace ERPLite.Web.Mapping
{
    public class SalesWebMappingProfile : Profile
    {
        public SalesWebMappingProfile()
        {
            CreateMap<CreateCustomerViewModel, CreateCustomerDto>();
            CreateMap<EditCustomerViewModel, UpdateCustomerDto>();
            CreateMap<CustomerDto, CustomerDetailsViewModel>()
                .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.OnboardedAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.OrdersPipeline, opt => opt.MapFrom(src => src.Orders));
            CreateMap<CustomerDto, EditCustomerViewModel>();

            CreateMap<CreateOrderViewModel, CreateOrderDto>();
            CreateMap<OrderItemViewModel, CreateOrderItemDto>();
            CreateMap<OrderDto, CreateOrderViewModel>()
                .ForMember(dest => dest.GrandTotal, opt => opt.MapFrom(src => src.TotalPrice))
                .ForMember(dest => dest.Customers, opt => opt.Ignore())
                .ForMember(dest => dest.Products, opt => opt.Ignore());

            CreateMap<PaymentDto, PaymentsIndexViewModel>();

            CreateMap<PaymentDto, PaymentItemViewModel>();

            CreateMap<CreatePaymentViewModel, CreatePaymentDto>();
        }
    }
}
