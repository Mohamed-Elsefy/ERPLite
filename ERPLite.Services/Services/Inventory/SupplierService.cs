using AutoMapper;
using ERPLite.Data.Entities.Inventory;
using ERPLite.Repositories.Interfaces.Common;
using ERPLite.Services.DTOs.Inventory;
using ERPLite.Services.Helpers;
using ERPLite.Services.Interfaces.Inventory;
using ERPLite.Services.Interfaces.System;
using ERPLite.Shared;
using ERPLite.Shared.Constants;

namespace ERPLite.Services.Services.Inventory
{
    public class SupplierService : ISupplierService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IActivityLogService _activityLogService;
        private readonly ICurrentUserService _currentUser;

        public SupplierService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IActivityLogService activityLogService,
            ICurrentUserService currentUser)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _activityLogService = activityLogService;
            _currentUser = currentUser;
        }

        public async Task<ServiceResult<IEnumerable<SupplierDto>>> GetAllAsync()
        {
            var suppliers = await _unitOfWork.Suppliers.GetAllAsync();
            var result = _mapper.Map<IEnumerable<SupplierDto>>(suppliers);

            return ServiceResult<IEnumerable<SupplierDto>>.Successful(result);
        }

        public async Task<ServiceResult<SupplierDto>> GetByIdAsync(int id)
        {
            if (id <= 0)
                return ServiceResult<SupplierDto>
                    .Failed("Invalid supplier id.");
            var supplier = await _unitOfWork.Suppliers.GetByIdAsync(id);
            if (supplier == null)
                return ServiceResult<SupplierDto>.Failed("Supplier not found.");

            var dto = _mapper.Map<SupplierDto>(supplier);
            return ServiceResult<SupplierDto>.Successful(dto);
        }

        public async Task<ServiceResult> CreateAsync(CreateSupplierDto dto)
        {
           dto.Name = dto.Name.Trim();

            if (string.IsNullOrWhiteSpace(dto.Name))
                return ServiceResult.Failed("Supplier name is required.");

            var exists = await _unitOfWork.Suppliers.SupplierExistsAsync(dto.Name);
            if (exists)
                return ServiceResult.Failed("Supplier already exists.");
            var user = _currentUser.UserId ?? "System";

            var supplier = _mapper.Map<Supplier>(dto);

            await _unitOfWork.Suppliers.AddAsync(supplier);
            await _unitOfWork.SaveChangesAsync();

            await _activityLogService.LogAsync(
                userId: user,
                action: "Create",
                entityName: SystemModules.Suppliers,
                entityId: supplier.Id,
                description: $"Created supplier profile for '{supplier.Name}' with contact information."
            );

            return ServiceResult.Successful("Supplier created successfully.");
        }

        public async Task<ServiceResult> UpdateAsync(UpdateSupplierDto dto)
        {
            dto.Name = dto.Name.Trim();
            var supplier = await _unitOfWork.Suppliers.GetByIdAsync(dto.Id);
            if (supplier == null)
                return ServiceResult.Failed("Supplier not found.");

            var exists = await _unitOfWork.Suppliers.SupplierExistsAsync(dto.Name, dto.Id);
            if (exists)
                return ServiceResult.Failed("Supplier name already exists.");

            _mapper.Map(dto, supplier);
            var user = _currentUser.UserId ?? "System";

            _unitOfWork.Suppliers.Update(supplier);
            await _unitOfWork.SaveChangesAsync();

            await _activityLogService.LogAsync(
                userId: user,
                action: "Update",
                entityName: SystemModules.Suppliers,
                entityId: supplier.Id,
                description: $"Updated profile and supplier data for: '{supplier.Name}'."
            );

            return ServiceResult.Successful("Supplier updated successfully.");
        }

        public async Task<ServiceResult> DeleteAsync(int id)
        {
            if (id <= 0)
                return ServiceResult<SupplierDto>
                    .Failed("Invalid supplier id.");
            var supplier = await _unitOfWork.Suppliers.GetByIdAsync(id);
            if (supplier == null)
                return ServiceResult.Failed("Supplier not found.");

            var hasProducts = await _unitOfWork.Suppliers.HasProductsAsync(id);
            if (hasProducts)
                return ServiceResult.Failed("Cannot delete supplier with products.");

            var supplierName = supplier.Name;
            var user = _currentUser.UserId ?? "System";
            _unitOfWork.Suppliers.SoftDelete(supplier);
            await _unitOfWork.SaveChangesAsync();

            await _activityLogService.LogAsync(
                userId: user,
                action: "Delete",
                entityName: SystemModules.Suppliers,
                entityId: id,
                description: $"Soft-deleted supplier record: '{supplierName}'."
            );

            return ServiceResult.Successful("Supplier deleted successfully.");
        }
    }
}