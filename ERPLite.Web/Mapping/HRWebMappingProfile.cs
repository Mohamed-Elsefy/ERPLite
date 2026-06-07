using AutoMapper;
using ERPLite.Services.DTOs.HR;
using ERPLite.Web.Models.Departments;
using ERPLite.Web.Models.Employees;

namespace ERPLite.Web.Mapping
{
    public class HRWebMappingProfile : Profile
    {
        public HRWebMappingProfile()
        {

            CreateMap<DepartmentDto, DepartmentDetailsViewModel>()
                .ForMember(dest => dest.DepartmentId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.EmployeesCount, opt => opt.MapFrom(src => src.Employees != null ? src.Employees.Count : 0))
                .ForMember(dest => dest.Employees, opt => opt.MapFrom(src => src.Employees));


            CreateMap<EmployeeDto, EmployeeDetailsViewModel>()
                .ForMember(dest => dest.EmployeeId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.AttendanceLogs, opt => opt.Ignore());

            CreateMap<EmployeeDto, EditEmployeeViewModel>()
                .ForMember(dest => dest.Employee, opt => opt.MapFrom(src => new UpdateEmployeeDto
                {
                    Id = src.Id,
                    FullName = src.FullName,
                    Email = src.Email,
                    Salary = src.Salary,
                    DepartmentId = src.DepartmentId,
                    UserId = src.UserId
                }))
                .ForMember(dest => dest.DepartmentList, opt => opt.Ignore());
        }
    }
}
