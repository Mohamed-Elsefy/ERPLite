using AutoMapper;
using ERPLite.Data.Entities.HR;
using ERPLite.Repositories.Interfaces.Common;
using ERPLite.Services.DTOs.HR;
using ERPLite.Services.Helpers;
using ERPLite.Services.Interfaces.HR;
using ERPLite.Services.Interfaces.System;
using ERPLite.Shared.Constants;

namespace ERPLite.Services.Services.HR
{
    public class AttendanceService : IAttendanceService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IActivityLogService _activityLogService;

        public AttendanceService(IUnitOfWork unitOfWork, IMapper mapper, IActivityLogService activityLogService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _activityLogService = activityLogService;
        }

        public async Task<ServiceResult> CheckInAsync(CheckInDto dto, string currentUserId)
        {
            var employee = await _unitOfWork.Employees.GetByIdAsync(dto.EmployeeId);
            if (employee is null)
                return ServiceResult.Failed("Employee not found.");

            var exists = await _unitOfWork.Attendances.HasAttendanceTodayAsync(dto.EmployeeId);
            if (exists)
                return ServiceResult.Failed("Attendance already recorded today.");

            var attendance = _mapper.Map<Attendance>(dto);

            await _unitOfWork.Attendances.AddAsync(attendance);
            await _unitOfWork.SaveChangesAsync();

            await _activityLogService.LogAsync(
                userId: currentUserId,
                action: "CheckIn",
                entityName: "Attendance",
                entityId: attendance.Id,
                description: $"Recorded Check-In for employee: '{employee.FullName}' at {attendance.CheckInTime}."
            );

            return ServiceResult.Successful("Check-in recorded successfully.");
        }

        public async Task<ServiceResult> CheckOutAsync(CheckOutDto dto, string currentUserId)
        {
            var attendance = await _unitOfWork.Attendances.GetAttendanceByDateAsync(dto.EmployeeId, DateTime.Today);
            if (attendance is null)
                return ServiceResult.Failed("No check-in found today.");

            if (attendance.CheckOutTime.HasValue)
                return ServiceResult.Failed("Employee already checked out.");

            attendance.CheckOutTime = DateTime.Now;

            _unitOfWork.Attendances.Update(attendance);
            await _unitOfWork.SaveChangesAsync();

            var employee = await _unitOfWork.Employees.GetByIdAsync(dto.EmployeeId);
            var employeeName = employee?.FullName ?? $"ID {dto.EmployeeId}";

            await _activityLogService.LogAsync(
                userId: currentUserId,
                action: "CheckOut",
                entityName: "Attendance",
                entityId: attendance.Id,
                description: $"Recorded Check-Out for employee: '{employeeName}' at {attendance.CheckOutTime}."
            );

            return ServiceResult.Successful("Check-out recorded successfully.");
        }

        public async Task<ServiceResult<IEnumerable<AttendanceDto>>> GetEmployeeAttendanceAsync(int employeeId)
        {
            var records = await _unitOfWork.Attendances.GetEmployeeAttendanceAsync(employeeId);
            var result = _mapper.Map<IEnumerable<AttendanceDto>>(records);

            return ServiceResult<IEnumerable<AttendanceDto>>.Successful(result);
        }

        public async Task<ServiceResult<IEnumerable<AttendanceDto>>> GetTodayAttendanceAsync()
        {
            var records = await _unitOfWork.Attendances.GetTodayAttendanceAsync();
            var result = _mapper.Map<IEnumerable<AttendanceDto>>(records);

            return ServiceResult<IEnumerable<AttendanceDto>>.Successful(result);
        }
    }
}