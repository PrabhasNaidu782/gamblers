using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GamblersGrocery.Models.Entities
{
    public class Product
    {
        [Key] public int productId { get; set; }
        [Required(ErrorMessage = "Product name is required")][MaxLength(100)][Display(Name = "Product Name")]
        public string productName { get; set; } = string.Empty;
        [MaxLength(50)][Display(Name = "Barcode")]
        public string barcode { get; set; } = string.Empty;
        [Required][Column(TypeName = "decimal(10,2)")][Range(0.01, 999999)][Display(Name = "Price (Rs)")]
        public decimal price { get; set; }
        [Required(ErrorMessage = "Category is required")][MaxLength(50)][Display(Name = "Category")]
        public string category { get; set; } = string.Empty;
        [Required][Range(0, 999999)][Display(Name = "Stock Quantity")]
        public int stockQuantity { get; set; }
        public ICollection<StockLog> StockLogs { get; set; } = new List<StockLog>();
        public ICollection<Promotion> Promotions { get; set; } = new List<Promotion>();
    }
}
