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
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly IActivityLogService activityLogService;
        private readonly INotificationService notificationService;

        public SupplierService(IUnitOfWork unitOfWork, IMapper mapper, IActivityLogService activityLogService, INotificationService notificationService)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.activityLogService = activityLogService;
            this.notificationService = notificationService;
        }

        public async Task<ServiceResult<IEnumerable<SupplierDto>>> GetAllAsync()
        {
            var suppliers = await unitOfWork.Suppliers.GetAllAsync();
            var result = mapper.Map<IEnumerable<SupplierDto>>(suppliers);

            return ServiceResult<IEnumerable<SupplierDto>>.Successful(result);
        }

        public async Task<ServiceResult<SupplierDto>> GetByIdAsync(int id)
        {
            var supplier = await unitOfWork.Suppliers.GetByIdAsync(id);
            if (supplier == null)
                return ServiceResult<SupplierDto>.Failed("Supplier not found.");

            var dto = mapper.Map<SupplierDto>(supplier);
            return ServiceResult<SupplierDto>.Successful(dto);
        }

        public async Task<ServiceResult> CreateAsync(CreateSupplierDto dto, string currentUserId)
        {
            var exists = await unitOfWork.Suppliers.SupplierExistsAsync(dto.Name);
            if (exists)
                return ServiceResult.Failed("Supplier already exists.");

            var supplier = mapper.Map<Supplier>(dto);

            await unitOfWork.Suppliers.AddAsync(supplier);

            await notificationService.CreateSystemNotificationAsync(
                userId: currentUserId,
                title: "Supply Chain Expansion",
                message: $"Vendor profile for '{supplier.Name}' registered successfully into global procurement tracking system.",
                type: "Procurement",
                priority: "Medium"
            );

            await unitOfWork.SaveChangesAsync();

            await activityLogService.LogAsync(
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
            var supplier = await unitOfWork.Suppliers.GetByIdAsync(dto.Id);
            if (supplier == null)
                return ServiceResult.Failed("Supplier not found.");

            var exists = await unitOfWork.Suppliers.SupplierExistsAsync(dto.Name, dto.Id);
            if (exists)
                return ServiceResult.Failed("Supplier name already exists.");

            mapper.Map(dto, supplier);

            unitOfWork.Suppliers.Update(supplier);
            await unitOfWork.SaveChangesAsync();

            await activityLogService.LogAsync(
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
            var supplier = await unitOfWork.Suppliers.GetByIdAsync(id);
            if (supplier == null)
                return ServiceResult.Failed("Supplier not found.");

            var hasProducts = await unitOfWork.Suppliers.HasProductsAsync(id);
            if (hasProducts)
                return ServiceResult.Failed("Cannot delete supplier with products.");

            var supplierName = supplier.Name;

            unitOfWork.Suppliers.SoftDelete(supplier);
            await unitOfWork.SaveChangesAsync();

            await activityLogService.LogAsync(
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