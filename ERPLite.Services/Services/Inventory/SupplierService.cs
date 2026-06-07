using AutoMapper;
using ERPLite.Data.Entities.Inventory;
using ERPLite.Repositories.Interfaces.Common;
using ERPLite.Services.DTOs.Inventory;
using ERPLite.Services.Helpers;
using ERPLite.Services.Interfaces.Inventory;
using ERPLite.Services.Interfaces.System;
using ERPLite.Shared.Constants;

namespace ERPLite.Services.Services.Inventory
{
    public class SupplierService : ISupplierService
    {
        private readonly IUnitOfWork _unitOfWork; 
        private readonly IMapper _mapper;
        private readonly IActivityLogService _activityLogService;
        private readonly INotificationService _notificationService;

        public SupplierService(IUnitOfWork unitOfWork, IMapper mapper, IActivityLogService activityLogService, INotificationService notificationService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _activityLogService = activityLogService;
            _notificationService = notificationService;
        }

        public async Task<ServiceResult<IEnumerable<SupplierDto>>> GetAllAsync()
        {
            var suppliers = await _unitOfWork.Suppliers.GetAllAsync();
            var result = _mapper.Map<IEnumerable<SupplierDto>>(suppliers);
            return ServiceResult<IEnumerable<SupplierDto>>.Successful(result);
        }

        public async Task<ServiceResult<SupplierDto>> GetByIdAsync(int id)
        {
            // 🌟 جلب المورد مدمجاً بمنتجاته لحل مشاكل الأداء
            var supplier = await _unitOfWork.Suppliers.GetByIdAsync(id);
            if (supplier == null)
                return ServiceResult<SupplierDto>.Failed("Supplier not found.");

            var dto = _mapper.Map<SupplierDto>(supplier);
            return ServiceResult<SupplierDto>.Successful(dto);
        }

        public async Task<ServiceResult> CreateAsync(CreateSupplierDto dto, string currentUserId)
        {
            if (await _unitOfWork.Suppliers.SupplierExistsAsync(dto.Name))
                return ServiceResult.Failed("Supplier already exists.");

            var supplier = _mapper.Map<Supplier>(dto);
            await _unitOfWork.Suppliers.AddAsync(supplier);

            // 🌟 التعديل المعماري للحفظ أولاً
            await _unitOfWork.SaveChangesAsync();

            await _notificationService.CreateSystemNotificationAsync(
                userId: currentUserId,
                title: "Supply Chain Expansion",
                message: $"Vendor profile for '{supplier.Name}' registered successfully into global procurement tracking system.",
                type: "Procurement",
                priority: "Medium"
            );

            await _activityLogService.LogAsync(
                userId: currentUserId,
                action: "Create",
                entityName: SystemModules.Suppliers,
                entityId: supplier.Id,
                description: $"Created supplier profile for '{supplier.Name}' with contact information."
            );

            return ServiceResult.Successful("Supplier created successfully.");
        }

        public async Task<ServiceResult> UpdateAsync(UpdateSupplierDto dto, string currentUserId)
        {
            var supplier = await _unitOfWork.Suppliers.GetByIdAsync(dto.Id);
            if (supplier == null)
                return ServiceResult.Failed("Supplier not found.");

            if (await _unitOfWork.Suppliers.SupplierExistsAsync(dto.Name, dto.Id))
                return ServiceResult.Failed("Supplier name already exists.");

            _mapper.Map(dto, supplier);
            _unitOfWork.Suppliers.Update(supplier);
            await _unitOfWork.SaveChangesAsync();

            await _activityLogService.LogAsync(
                userId: currentUserId,
                action: "Update",
                entityName: SystemModules.Suppliers,
                entityId: supplier.Id,
                description: $"Updated profile and supplier data for: '{supplier.Name}'."
            );

            return ServiceResult.Successful("Supplier updated successfully.");
        }

        public async Task<ServiceResult> DeleteAsync(int id, string currentUserId)
        {
            var supplier = await _unitOfWork.Suppliers.GetByIdAsync(id);
            if (supplier == null)
                return ServiceResult.Failed("Supplier not found.");

            if (await _unitOfWork.Suppliers.HasProductsAsync(id))
                return ServiceResult.Failed("Cannot delete supplier with products.");

            var supplierName = supplier.Name;
            _unitOfWork.Suppliers.SoftDelete(supplier);
            await _unitOfWork.SaveChangesAsync();

            await _activityLogService.LogAsync(
                userId: currentUserId,
                action: "Delete",
                entityName: SystemModules.Suppliers,
                entityId: id,
                description: $"Soft-deleted supplier record: '{supplierName}'."
            );

            return ServiceResult.Successful("Supplier deleted successfully.");
        }
    }
}