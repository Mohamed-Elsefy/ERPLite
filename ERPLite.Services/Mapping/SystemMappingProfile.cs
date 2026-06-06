using AutoMapper;
using ERPLite.Data.Entities.System;
using ERPLite.Services.DTOs.System;

namespace ERPLite.Services.Mapping
{
    public class SystemMappingProfile : Profile
    {
        public SystemMappingProfile()
        {
            CreateMap<ActivityLog, ActivityLogDto>()
                .ForMember(d => d.UserFullName, o => o.MapFrom(s => s.User!.FullName));
        }
    }
}
