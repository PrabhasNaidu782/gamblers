using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GamblersGrocery.Models.Entities
{
    public class Transaction
    {
        [Key] public int transactionId { get; set; }
        public int cashierId { get; set; }
        public string cashierName { get; set; } = string.Empty;

        [Column(TypeName = "decimal(15,2)")] 
        public decimal totalAmount { get; set; }

        [Column(TypeName = "decimal(15,2)")] 
        public decimal discountAmount { get; set; }
        [Column(TypeName = "decimal(15,2)")] public decimal finalAmount { get; set; }
        [MaxLength(10)] public string paymentMode { get; set; } = "CASH";

        public string? upiId { get; set; }
        public DateTime transactionDate { get; set; } = DateTime.Now;
        public ICollection<TransactionItem> TransactionItems { get; set; } = new List<TransactionItem>();
    }
}
