using System.ComponentModel.DataAnnotations;
using GamblersGrocery.Models.Entities;

namespace GamblersGrocery.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Enter a valid email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }

    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Full name is required")]
        //[RegularExpression(@"^[a-zA_Z]",ErrorMessage ="Please Enter Valid User Name")]
        [Display(Name = "Full Name")]
        [StringLength(100, MinimumLength = 2)]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Enter a valid email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Minimum 6 characters")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Role is required")]
        public string Role { get; set; } = string.Empty;
    }

    public class BillItemViewModel
    {
        public int productId { get; set; }
        public string productName { get; set; } = string.Empty;
        public string barcode { get; set; } = string.Empty;
        public decimal? unitPrice { get; set; }
        public int? quantity { get; set; }
        public decimal discountPercent { get; set; }
        public decimal productDiscount { get; set; }
        public decimal lineTotal { get; set; }

        public int availableStock { get; set; }
    }

    public class BillViewModel
    {
        public List<BillItemViewModel> Items { get; set; } = new();
        public decimal subTotal { get; set; }
        public decimal billDiscountPercent { get; set; }
        public decimal billDiscountAmount { get; set; }
        public decimal totalAmount { get; set; }
        public string paymentMode { get; set; } = "CASH";
        public string cashierName { get; set; } = string.Empty;
    }

    public class DailySalesReportViewModel
    {
        public DateTime reportDate { get; set; }
        public List<Transaction> Transactions { get; set; } = new();
        public decimal totalSales { get; set; }
        public int totalTransactions { get; set; }
        public decimal totalDiscount { get; set; }
        public Dictionary<string, int> PaymentBreakdown { get; set; } = new();
    }

    public class AdminDashboardViewModel
    {
        public decimal TodaySales { get; set; }
        public int TodayTransactions { get; set; }
        public int TotalProducts { get; set; }
        public int LowStockCount { get; set; }
        public int ActivePromotions { get; set; }
        public List<Product> LowStockProducts { get; set; } = new();
        public List<Transaction> RecentTransactions { get; set; } = new();
    }

    public class CloseDayViewModel
    {
        public DateTime date { get; set; } = DateTime.Today;
        [Range(0, double.MaxValue, ErrorMessage = "Enter a valid amount")]
        [Display(Name = "Cash In Hand (Rs)")]
        public decimal cashInHand { get; set; }
        public decimal systemTotal { get; set; }
        public string cashierName { get; set; } = string.Empty;
    }
}
