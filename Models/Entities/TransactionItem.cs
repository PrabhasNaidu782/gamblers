using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GamblersGrocery.Models.Entities
{
    public class TransactionItem
    {
        [Key] public int itemId { get; set; }
        public int transactionId { get; set; }
        [ForeignKey("transactionId")] public Transaction? Transaction { get; set; }
        public int productId { get; set; }
        [ForeignKey("productId")] public Product? Product { get; set; }
        public int quantity { get; set; }
        [Column(TypeName = "decimal(10,2)")] public decimal unitPrice { get; set; }
        [Column(TypeName = "decimal(10,2)")] public decimal productDiscount { get; set; }
        [Column(TypeName = "decimal(10,2)")] public decimal lineTotal { get; set; }
    }
}
