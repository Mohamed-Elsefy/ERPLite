using AutoMapper;
using ERPLite.Data.Entities.Inventory;
using ERPLite.Repositories.Interfaces.Common;
using ERPLite.Services.DTOs.Inventory;
using ERPLite.Services.Helpers;
using ERPLite.Services.Interfaces.Inventory;
using ERPLite.Services.Interfaces.System;
using ERPLite.Shared.Constants;
using FluentValidation;

namespace ERPLite.Services.Services.Inventory
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IActivityLogService _activityLogService;
        private readonly INotificationService _notificationService;
        private readonly IValidator<CreateCategoryDto> _createValidator;
        private readonly IValidator<UpdateCategoryDto> _updateValidator;

        public CategoryService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IActivityLogService activityLogService,
            INotificationService notificationService,
            IValidator<CreateCategoryDto> createValidator,
            IValidator<UpdateCategoryDto> updateValidator)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _activityLogService = activityLogService;
            _notificationService = notificationService;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        public async Task<ServiceResult<IEnumerable<CategoryDto>>> GetAllAsync()
        {
            var categories = await _unitOfWork.Categories.GetAllAsync();
            var result = _mapper.Map<IEnumerable<CategoryDto>>(categories);
            return ServiceResult<IEnumerable<CategoryDto>>.Successful(result);
        }

        public async Task<ServiceResult<CategoryDto>> GetByIdAsync(int id)
        {
            var category = await _unitOfWork.Categories.GetCategoryWithProductsAsync(id);
            if (category == null)
                return ServiceResult<CategoryDto>.Failed("Category not found.");

            var dto = _mapper.Map<CategoryDto>(category);
            return ServiceResult<CategoryDto>.Successful(dto);
        }

        public async Task<ServiceResult> CreateAsync(CreateCategoryDto dto, string currentUserId)
        {
            var validationResult = await _createValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
                return ServiceResult.Failed(validationResult.ToErrorString());

            if (await _unitOfWork.Categories.CategoryExistsAsync(dto.Name))
                return ServiceResult.Failed("Category already exists.");

            var category = _mapper.Map<Category>(dto);
            await _unitOfWork.Categories.AddAsync(category);

            await _unitOfWork.SaveChangesAsync();

            await _notificationService.CreateSystemNotificationAsync(
                userId: currentUserId,
                title: "New Inventory Node Structure",
                message: $"A new structural classification group '{category.Name}' has been introduced to the catalog matrix.",
                type: NotificationTypes.Catalog,
                priority: NotificationPriorities.Low
            );

            await _activityLogService.LogAsync(
                userId: currentUserId,
                action: AuditActions.Create,
                entityName: SystemModules.Categories,
                entityId: category.Id,
                description: $"Created inventory category group: '{category.Name}'."
            );

            return ServiceResult.Successful("Category created successfully.");
        }

        public async Task<ServiceResult> UpdateAsync(UpdateCategoryDto dto, string currentUserId)
        {
            var validationResult = await _updateValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
                return ServiceResult.Failed(validationResult.ToErrorString());

            var category = await _unitOfWork.Categories.GetByIdAsync(dto.Id);
            if (category == null)
                return ServiceResult.Failed("Category not found.");

            if (await _unitOfWork.Categories.CategoryExistsAsync(dto.Name, dto.Id))
                return ServiceResult.Failed("Category name already exists.");

            _mapper.Map(dto, category);
            _unitOfWork.Categories.Update(category);
            await _unitOfWork.SaveChangesAsync();

            await _activityLogService.LogAsync(
                userId: currentUserId,
                action: AuditActions.Update,
                entityName: SystemModules.Categories,
                entityId: category.Id,
                description: $"Updated inventory category properties for: '{category.Name}'."
            );

            return ServiceResult.Successful("Category updated successfully.");
        }

        public async Task<ServiceResult> DeleteAsync(int id, string currentUserId)
        {
            var category = await _unitOfWork.Categories.GetCategoryWithProductsAsync(id);
            if (category == null)
                return ServiceResult.Failed("Category not found.");

            if (category.Products != null && category.Products.Any())
                return ServiceResult.Failed("Cannot delete category with products.");

            var categoryName = category.Name;
            _unitOfWork.Categories.SoftDelete(category);
            await _unitOfWork.SaveChangesAsync();

            await _activityLogService.LogAsync(
                userId: currentUserId,
                action: AuditActions.Delete,
                entityName: SystemModules.Categories,
                entityId: id,
                description: $"Soft-deleted inventory category group: '{categoryName}'."
            );

            return ServiceResult.Successful("Category deleted successfully.");
        }
    }
}