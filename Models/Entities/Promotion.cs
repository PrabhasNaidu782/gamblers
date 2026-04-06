using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GamblersGrocery.Models.Entities
{
    public class Promotion
    {
        [Key] public int promotionId { get; set; }
        [Required(ErrorMessage = "Please select a product")][Display(Name = "Product")]
        public int productId { get; set; }
        [ForeignKey("productId")] public Product? Product { get; set; }
        [Required][Column(TypeName = "decimal(5,2)")][Range(0.01, 99.99, ErrorMessage = "Discount must be 0.01-99.99")][Display(Name = "Discount (%)")]
        public decimal? discountPercent { get; set; }
        [Required][DataType(DataType.Date)][Display(Name = "Start Date")]
        public DateTime startDate { get; set; }
        [Required][DataType(DataType.Date)][Display(Name = "End Date")]
        public DateTime endDate { get; set; }
    }
}
