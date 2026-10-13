using AutoMapper;
using ERPLite.Data.Entities.HR;
using ERPLite.Services.DTOs.Dashboard;
using ERPLite.Services.DTOs.HR;
using ERPLite.Shared.Constants;
using System.Collections.Generic;

namespace ERPLite.Services.Mapping
{
    public class HRMappingProfile : Profile
    {
        public HRMappingProfile()
        {
            CreateMap<Department, DepartmentDto>()
                .ForMember(d => d.EmployeesCount, o => o.MapFrom(s => s.Employees.Count))
                .ForMember(d => d.Employees, o => o.MapFrom(s => s.Employees));

            CreateMap<CreateDepartmentDto, Department>();
            CreateMap<UpdateDepartmentDto, Department>();

            CreateMap<Employee, EmployeeDto>()
                .ForMember(d => d.DepartmentName, o => o.MapFrom(s => s.Department.Name));

            CreateMap<CreateEmployeeDto, Employee>();
            CreateMap<UpdateEmployeeDto, Employee>();


            CreateMap<Attendance, AttendanceDto>()
                .ForMember(d => d.EmployeeName, o => o.MapFrom(s => s.Employee != null ? s.Employee.FullName : string.Empty))
                .ForMember(d => d.Status, o => o.MapFrom(s => s.Status.ToString()));

            CreateMap<Attendance, AttendanceManagementDto>()
                .ForMember(d => d.AttendanceId, o => o.MapFrom(s => s.Id))
                .ForMember(d => d.EmployeeName, o => o.MapFrom(s => s.Employee != null ? s.Employee.FullName : DisplayDefaults.UnknownEmployee))
                .ForMember(d => d.DepartmentName, o => o.MapFrom(s => s.Employee != null && s.Employee.Department != null ? s.Employee.Department.Name : DisplayDefaults.NoDepartment))
                .ForMember(d => d.Status, o => o.MapFrom(s => s.Status.ToString()));
        }
    }
}