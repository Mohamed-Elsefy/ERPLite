using AutoMapper;
using ERPLite.Data.Entities.System;
using ERPLite.Repositories.Interfaces.Common;
using ERPLite.Services.DTOs.System;
using ERPLite.Services.Helpers;
using ERPLite.Services.Interfaces.System;
using ERPLite.Services.Interfaces.Infrastructure;

namespace ERPLite.Services.Services.System
{
    public class ActivityLogService : IActivityLogService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IDateTimeProvider _dateTimeProvider; 

        public ActivityLogService(IUnitOfWork unitOfWork, IMapper mapper, IDateTimeProvider dateTimeProvider)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task LogAsync(string userId, string action, string entityName, int entityId, string description)
        {
            var log = new ActivityLog
            {
                UserId = userId,
                Action = action,
                EntityName = entityName,
                EntityId = entityId,
                Description = description,
                Timestamp = _dateTimeProvider.UtcNow 
            };

            await _unitOfWork.ActivityLogs.AddAsync(log);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<ServiceResult<IEnumerable<ActivityLogDto>>> GetRecentLogsAsync()
        {
            var logs = await _unitOfWork.ActivityLogs.GetRecentLogsAsync(100);
            var dto = _mapper.Map<IEnumerable<ActivityLogDto>>(logs);
            return ServiceResult<IEnumerable<ActivityLogDto>>.Successful(dto);
        }

        public async Task<ServiceResult<ActivityLogDto>> GetLogByIdAsync(int id)
        {
            var log = await _unitOfWork.ActivityLogs.GetByIdAsync(id);
            if (log == null)
                return ServiceResult<ActivityLogDto>.Failed("Activity log instance not found.");

            var dto = _mapper.Map<ActivityLogDto>(log);
            return ServiceResult<ActivityLogDto>.Successful(dto);
        }

        public async Task<ServiceResult<IEnumerable<ActivityLogDto>>> GetUserLogsAsync(string userId)
        {
            var logs = await _unitOfWork.ActivityLogs.GetByUserAsync(userId);
            var dto = _mapper.Map<IEnumerable<ActivityLogDto>>(logs);
            return ServiceResult<IEnumerable<ActivityLogDto>>.Successful(dto);
        }

        public async Task<ServiceResult<IEnumerable<ActivityLogDto>>> GetEntityLogsAsync(string entityName, int entityId)
        {
            var logs = await _unitOfWork.ActivityLogs.GetByEntityAsync(entityName, entityId);
            var dto = _mapper.Map<IEnumerable<ActivityLogDto>>(logs);
            return ServiceResult<IEnumerable<ActivityLogDto>>.Successful(dto);
        }
    }
}