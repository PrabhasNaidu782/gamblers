using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GamblersGrocery.Models.Entities
{
    public class StockLog
    {
        [Key] public int logId { get; set; }
        public int productId { get; set; }
        [ForeignKey("productId")] public Product? Product { get; set; }
        public int quantityChanged { get; set; }
        [MaxLength(30)] public string changeType { get; set; } = string.Empty;
        public DateTime timestamp { get; set; } = DateTime.Now;
    }
}
