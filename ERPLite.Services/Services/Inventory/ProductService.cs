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
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IActivityLogService _activityLogService;
        private readonly ICurrentUserService _currentUser;

        public ProductService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IActivityLogService activityLogService, ICurrentUserService currentUser)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _activityLogService = activityLogService;
            _currentUser = currentUser;
        }

        public async Task<ServiceResult<IEnumerable<ProductDto>>> GetAllAsync()
        {
            var products = await _unitOfWork.Products.GetAllWithDetailsAsync();
            var result = _mapper.Map<IEnumerable<ProductDto>>(products);

            return ServiceResult<IEnumerable<ProductDto>>.Successful(result);
        }

        public async Task<ServiceResult<ProductDto>> GetByIdAsync(int id)
        {
            var product = await _unitOfWork.Products.GetByIdWithDetailsAsync(id);
            if (product == null)
                return ServiceResult<ProductDto>.Failed("Product not found.");

            var dto = _mapper.Map<ProductDto>(product);
            return ServiceResult<ProductDto>.Successful(dto);
        }

        public async Task<ServiceResult<IEnumerable<ProductDto>>> GetLowStockProductsAsync()
        {
            var products = await _unitOfWork.Products.GetLowStockProductsAsync();
            var result = _mapper.Map<IEnumerable<ProductDto>>(products);

            return ServiceResult<IEnumerable<ProductDto>>.Successful(result);
        }

        public async Task<ServiceResult> CreateAsync(CreateProductDto dto)
        {
            if (await _unitOfWork.Products.ExistsByNameAsync(dto.Name))
                return ServiceResult.Failed("Product already exists.");

            var category = await _unitOfWork.Categories.GetByIdAsync(dto.CategoryId);
            if (category == null)
                return ServiceResult.Failed("Category not found.");

            var supplier = await _unitOfWork.Suppliers.GetByIdAsync(dto.SupplierId);
            if (supplier == null)
                return ServiceResult.Failed("Supplier not found.");

            var product = _mapper.Map<Product>(dto);
            var user = _currentUser.UserId;
            await _unitOfWork.Products.AddAsync(product);
            await _unitOfWork.SaveChangesAsync();

            await _activityLogService.LogAsync(
                userId: user,
                action: "Create",
                entityName: SystemModules.Products,
                entityId: product.Id,
                description: $"Created product entry for '{product.Name}' with initial stock of {product.QuantityInStock} units under category '{category.Name}'."
            );

            return ServiceResult.Successful("Product created successfully.");
        }

        public async Task<ServiceResult> UpdateAsync(UpdateProductDto dto)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(dto.Id);
            if (product == null)
                return ServiceResult.Failed("Product not found.");

            var category = await _unitOfWork.Categories.GetByIdAsync(dto.CategoryId);
            if (category == null)
                return ServiceResult.Failed("Category not found.");

            var supplier = await _unitOfWork.Suppliers.GetByIdAsync(dto.SupplierId);
            if (supplier == null)
                return ServiceResult.Failed("Supplier not found.");

            if (await _unitOfWork.Products.ExistsByNameAsync(dto.Name, dto.Id))
                return ServiceResult.Failed("Product name already exists.");

            _mapper.Map(dto, product);
            var user = _currentUser.UserId;

            _unitOfWork.Products.Update(product);
            await _unitOfWork.SaveChangesAsync();

            await _activityLogService.LogAsync(
                userId: user,
                action: "Update",
                entityName: SystemModules.Products,
                entityId: product.Id,
                description: $"Updated catalog details and inventory rules for product: '{product.Name}'."
            );

            return ServiceResult.Successful("Product updated successfully.");
        }

        public async Task<ServiceResult> DeleteAsync(int id)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id);
            if (product == null)
                return ServiceResult.Failed("Product not found.");

            var usedInOrders = await _unitOfWork.Products.HasAnyOrdersAsync(id);
            if (usedInOrders)
                return ServiceResult.Failed("Cannot delete product used in orders.");

            var productName = product.Name;
            var user = _currentUser.UserId;

            _unitOfWork.Products.SoftDelete(product);
            await _unitOfWork.SaveChangesAsync();

            await _activityLogService.LogAsync(
                userId: user,
                action: "Delete",
                entityName: SystemModules.Products,
                entityId: id,
                description: $"Soft-deleted product from inventory tracking: '{productName}'."
            );

            return ServiceResult.Successful("Product deleted successfully.");
        }
    }
}