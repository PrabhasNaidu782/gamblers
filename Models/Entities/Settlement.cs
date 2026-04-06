using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GamblersGrocery.Models.Entities
{
    public class Settlement
    {
        [Key] public int settlementId { get; set; }
        public int cashierId { get; set; }
        public string cashierName { get; set; } = string.Empty;
        [Column(TypeName = "decimal(15,2)")] public decimal totalSales { get; set; }
        [Column(TypeName = "decimal(15,2)")] public decimal cashInHand { get; set; }
        [Column(TypeName = "decimal(15,2)")] public decimal variance { get; set; }
        public DateTime settlementDate { get; set; }
    }
}
