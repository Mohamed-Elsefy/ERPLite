using AutoMapper;
using ERPLite.Data.Entities.Identity;
using ERPLite.Services.DTOs.Users;

namespace ERPLite.Services.Mapping
{
    public class AuthMappingProfile : Profile
    {
        public AuthMappingProfile()
        {
            CreateMap<ApplicationUser, UserDto>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id)) 
                .ForMember(dest => dest.EmployeeId, opt => opt.MapFrom(src => src.EmployeeId)) 
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt));

            CreateMap<CreateUserDto, ApplicationUser>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));
        }
    }
}
