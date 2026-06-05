using AutoMapper;
using ERPLite.Data.Entities.Inventory;
using ERPLite.Repositories.Interfaces.Common;
using ERPLite.Services.DTOs.Inventory;
using ERPLite.Services.Helpers;
using ERPLite.Services.Interfaces.Inventory;
using ERPLite.Shared;

namespace ERPLite.Services.Services.Inventory
{
    public class InventoryService : IInventoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUser;
        private readonly IGenericRepository<StockMovement,int> _stockMoveRepo;

        public InventoryService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ICurrentUserService currentUser,
            IGenericRepository<StockMovement,int> stockmoveRepo)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _currentUser = currentUser;
            _stockMoveRepo = stockmoveRepo;
        }
        public async Task<ServiceResult> AdjustStockAsync(int productId, int quantity, string? notes = null)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(productId);

            if (product == null)
                return ServiceResult.Failed("Product not found");

            var difference = quantity - product.QuantityInStock;

            product.QuantityInStock = quantity;

            await _stockMoveRepo.AddAsync(
                            new StockMovement
                            {
                                ProductId = productId,
                                Quantity = quantity,
                                Type = StockMovementType.Adjustment,
                                Notes = notes,
                            }); await _unitOfWork.SaveChangesAsync();

            return ServiceResult.Successful($"Product {product.Name} stock adjusted Successfully");
        }

        public async Task<ServiceResult<IEnumerable<StockMovementDto>>> GetHistoryAsync(int productId)
        {
           var history = await _stockMoveRepo.FindAsync(p=>p.Id == productId);
            if(history == null)
                return ServiceResult<IEnumerable<StockMovementDto>>.Failed("there is no history for this product");


            var result = history.OrderByDescending(x => x.CreatedAtUtc)
                .Select( x => new StockMovementDto
                {
                    Id = x.Id,
                    ProductId = x.ProductId,
                    CreatedAt =  x.CreatedAtUtc,
                    Notes = x.Notes,
                    ProductName =  _unitOfWork.Products.GetByIdAsync(x.ProductId).Result.Name,
                    Quantity = x.Quantity,
                    Type = x.Type

                });

            return ServiceResult<IEnumerable<StockMovementDto>>.Successful(result);


        }

        public async Task<ServiceResult> StockInAsync(int productId, int quantity, string? notes = null)
        {
            if (quantity <= 0)
                return ServiceResult.Failed("Quantity must be greater than zero");


            var product = await _unitOfWork.Products.GetByIdAsync(productId);

            if (product == null)
                return ServiceResult.Failed("Product not found");

            product.QuantityInStock += quantity;

            await _stockMoveRepo.AddAsync(
                new StockMovement
            {
                    ProductId = productId,
                    Quantity = quantity,
                    Type = StockMovementType.StockIn,
                    Notes = notes,
            });

            await _unitOfWork.SaveChangesAsync();
            return ServiceResult.Successful($"Product {product.Name} StockIn Successfully");

        }

        public async Task<ServiceResult> StockOutAsync(int productId, int quantity, string? notes = null)
        {
            if (quantity <= 0)
                return ServiceResult.Failed("Quantity must be greater than zero");

            var product = await _unitOfWork.Products.GetByIdAsync(productId);
            if(product == null)
                return ServiceResult.Failed("Product not found");


            if (quantity > product.QuantityInStock)
                return ServiceResult.Failed("Insufficient stock");

            product.QuantityInStock -= quantity;

            await _stockMoveRepo.AddAsync(
                new StockMovement
                {
                    ProductId = productId,
                    Quantity = quantity,
                    Type = StockMovementType.StockOut,
                    Notes = notes,
                });

            await _unitOfWork.SaveChangesAsync();
            return ServiceResult.Successful($"Product {product.Name} StockOut Successfully");
        }
    }
}
