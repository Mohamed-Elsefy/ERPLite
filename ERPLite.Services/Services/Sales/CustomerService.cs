using AutoMapper;
using ERPLite.Data.Entities.Sales;
using ERPLite.Repositories.Interfaces.Common;
using ERPLite.Services.DTOs.Sales;
using ERPLite.Services.Helpers;
using ERPLite.Services.Interfaces.Sales;
using ERPLite.Services.Interfaces.System;
using ERPLite.Shared.Constants;

namespace ERPLite.Services.Services.Sales
{
    public class CustomerService : ICustomerService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IActivityLogService _activityLogService;

        public CustomerService(IUnitOfWork unitOfWork, IMapper mapper, IActivityLogService activityLogService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _activityLogService = activityLogService;
        }

        public async Task<ServiceResult<IEnumerable<CustomerDto>>> GetAllAsync()
        {
            var customers = await _unitOfWork.Customers.GetActiveCustomersAsync();
            var result = _mapper.Map<IEnumerable<CustomerDto>>(customers);

            return ServiceResult<IEnumerable<CustomerDto>>.Successful(result);
        }

        public async Task<ServiceResult<CustomerDto>> GetByIdAsync(int id)
        {
            var customer = await _unitOfWork.Customers.GetByIdAsync(id);
            if (customer == null)
                return ServiceResult<CustomerDto>.Failed("Customer not found.");

            var dto = _mapper.Map<CustomerDto>(customer);
            return ServiceResult<CustomerDto>.Successful(dto);
        }

        public async Task<ServiceResult> CreateAsync(CreateCustomerDto dto, string currentUserId)
        {
            if (await _unitOfWork.Customers.CustomerExistsByPhoneAsync(dto.Phone))
                return ServiceResult.Failed("Phone number already exists.");

            var customer = _mapper.Map<Customer>(dto);
            customer.CreatedAt = DateTime.UtcNow;

            await _unitOfWork.Customers.AddAsync(customer);
            await _unitOfWork.SaveChangesAsync();

            await _activityLogService.LogAsync(
                userId: currentUserId,
                action: "Create",
                entityName: SystemModules.Customers,
                entityId: customer.Id,
                description: $"Created customer record for '{customer.FullName}' with phone: {customer.Phone}."
            );

            return ServiceResult.Successful("Customer created successfully.");
        }

        public async Task<ServiceResult> UpdateAsync(UpdateCustomerDto dto, string currentUserId)
        {
            var customer = await _unitOfWork.Customers.GetByIdAsync(dto.Id);
            if (customer == null)
                return ServiceResult.Failed("Customer not found.");

            if (await _unitOfWork.Customers.CustomerExistsByPhoneAsync(dto.Phone, dto.Id))
                return ServiceResult.Failed("Phone number is already registered to another customer.");

            _mapper.Map(dto, customer);

            _unitOfWork.Customers.Update(customer);
            await _unitOfWork.SaveChangesAsync();

            await _activityLogService.LogAsync(
                userId: currentUserId,
                action: "Update",
                entityName: SystemModules.Customers,
                entityId: customer.Id,
                description: $"Updated profile and information details for customer: '{customer.FullName}'."
            );

            return ServiceResult.Successful("Customer updated successfully.");
        }

        public async Task<ServiceResult> DeleteAsync(int id, string currentUserId)
        {
            var customer = await _unitOfWork.Customers.GetByIdAsync(id);
            if (customer == null)
                return ServiceResult.Failed("Customer not found.");

            if (await _unitOfWork.Customers.HasOrdersAsync(id))
                return ServiceResult.Failed("Cannot delete customer with orders.");

            var customerName = customer.FullName;

            _unitOfWork.Customers.SoftDelete(customer);
            await _unitOfWork.SaveChangesAsync();

            await _activityLogService.LogAsync(
                userId: currentUserId,
                action: "Delete",
                entityName: SystemModules.Customers,
                entityId: id,
                description: $"Soft-deleted customer account: '{customerName}'."
            );

            return ServiceResult.Successful("Customer deleted successfully.");
        }
    }
}