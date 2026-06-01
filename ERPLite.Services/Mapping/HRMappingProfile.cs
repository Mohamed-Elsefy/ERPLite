using AutoMapper;
using ERPLite.Data.Entities.HR;
using ERPLite.Services.DTOs.HR;
using ERPLite.Shared.Enums;

namespace ERPLite.Services.Mapping
{
    public class HRMappingProfile : Profile
    {
        public HRMappingProfile()
        {
            // Departments Mapping
            CreateMap<Department, DepartmentDto>()
                .ForMember(d => d.EmployeesCount, o => o.MapFrom(s => s.Employees.Count));

            CreateMap<CreateDepartmentDto, Department>();
            CreateMap<UpdateDepartmentDto, Department>();

            // Employees Mapping
            CreateMap<Employee, EmployeeDto>()
                .ForMember(d => d.DepartmentName, o => o.MapFrom(s => s.Department.Name));

            CreateMap<CreateEmployeeDto, Employee>();

            CreateMap<UpdateEmployeeDto, Employee>();

            CreateMap<Attendance, AttendanceDto>()
                .ForMember(d => d.EmployeeName, o => o.MapFrom(s => s.Employee.FullName))
                .ForMember(d => d.Status, o => o.MapFrom(s => s.Status.ToString()));

            CreateMap<CheckInDto, Attendance>()
                .ForMember(dest => dest.Date, opt => opt.MapFrom(src => DateTime.Today))
                .ForMember(dest => dest.CheckInTime, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src =>
                    DateTime.Now.TimeOfDay > new TimeSpan(9, 0, 0)
                        ? AttendanceStatus.Late
                        : AttendanceStatus.Present));
        }
    }
}