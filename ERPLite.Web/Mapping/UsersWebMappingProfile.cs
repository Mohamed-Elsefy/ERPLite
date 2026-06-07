using AutoMapper;
using ERPLite.Services.DTOs.Users;
using ERPLite.Web.Areas.Admin.Models.Users;

namespace ERPLite.Web.Mapping
{
    public class UsersWebMappingProfile : Profile
    {
        public UsersWebMappingProfile()
        {
            CreateMap<CreateUserViewModel, CreateUserDto>();

            CreateMap<UserDto, UserListViewModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.UserId));
        }
    }
}
