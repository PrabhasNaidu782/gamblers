using GamblersGrocery.Models.Entities;
using GamblersGrocery.Models.ViewModels;
using GamblersGrocery.Repositories.Interfaces;
using GamblersGrocery.Services.Interfaces;

namespace GamblersGrocery.Services.Implementations
{
	public class PosService : IPosService
	{
		private readonly IProductRepository _productRepo;
		private readonly ITransactionRepository _txRepo;
		private readonly IInventoryRepository _inventoryRepo;
		private readonly IPromotionRepository _promoRepo;
		private readonly ILogger<PosService> _logger;

		public PosService(
			IProductRepository productRepo,
			ITransactionRepository txRepo,
			IInventoryRepository inventoryRepo,
			IPromotionRepository promoRepo,
			ILogger<PosService> logger)
		{
			_productRepo = productRepo;
			_txRepo = txRepo;
			_inventoryRepo = inventoryRepo;
			_promoRepo = promoRepo;
			_logger = logger;
		}

		public async Task<Product?> ScanProductAsync(string barcode)
		{
			try
			{
				return await _productRepo.GetProductByBarcodeAsync(barcode);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "PosService Scan failed");
				throw;
			}
		}

		public async Task<BillViewModel> ApplyDiscountAsync(BillViewModel bill, decimal extraDiscountPercent)
		{
			try
			{
				decimal sub = 0;

				foreach (var item in bill.Items)
				{
					var promo = await _promoRepo.GetActivePromotionForProductAsync(item.productId);

					item.discountPercent = promo?.discountPercent ?? 0;
					item.productDiscount = Math.Round((item.unitPrice ?? 0) * item.quantity * item.discountPercent / 100, 2);
					item.lineTotal = Math.Round(((item.unitPrice ?? 0) * item.quantity) - item.productDiscount, 2);

					sub += item.lineTotal;
				}

				bill.subTotal = sub;
				bill.billDiscountPercent = extraDiscountPercent;
				bill.billDiscountAmount = Math.Round(sub * extraDiscountPercent / 100, 2);
				bill.totalAmount = Math.Round(sub - bill.billDiscountAmount, 2);

				return bill;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "PosService ApplyDiscount failed");
				throw;
			}
		}

		public async Task<Transaction> CompleteSaleAsync(BillViewModel bill, int cashierId, string cashierName, string paymentMode, string? upiId)
		{
			try
			{
				var transaction = new Transaction
				{
					cashierId = cashierId,
					cashierName = cashierName,
					totalAmount = bill.subTotal + bill.billDiscountAmount, // Total before extra bill discount
					discountAmount = bill.billDiscountAmount + bill.Items.Sum(i => i.productDiscount),
					finalAmount = bill.totalAmount,
					upiId = upiId,
					paymentMode = paymentMode,
					transactionDate = DateTime.Now
				};

				foreach (var item in bill.Items)
				{
					transaction.TransactionItems.Add(new TransactionItem
					{
						productId = item.productId,
						quantity = item.quantity,
						unitPrice = item.unitPrice,
						productDiscount = item.productDiscount,
						lineTotal = item.lineTotal
					});

					// Update stock for each item sold
					await _inventoryRepo.UpdateStockAsync(item.productId, -item.quantity, "SALE_UPDATE");
				}

				await _txRepo.AddTransactionAsync(transaction);
				return transaction;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "PosService CompleteSale failed");
				throw;
			}
		}

		public async Task<Transaction?> GetTransactionDetailsAsync(int id)
		{
			try
			{
				return await _txRepo.GetTransactionByIdAsync(id);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "PosService GetTxDetails failed");
				throw;
			}
		}

		public async Task<IEnumerable<Product>> GetCurrentStockLevelsAsync()
		{
			try
			{
				// Retrieves all products from the database for the search/selection tool
				return await _productRepo.GetAllProductsAsync();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "PosService GetCurrentStockLevels failed");
				return Enumerable.Empty<Product>();
			}
		}
	}
}