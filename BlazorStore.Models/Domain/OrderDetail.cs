using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlazorStore.Models.Domain
{
    public class OrderDetail : BaseEntity
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int OrderHeaderId { get; set; }
        [Required]
        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        [NotMapped]
        public Product? Product { get; set; }
        [Required]
        public int Count { get; set; }
        [Required]
        public double Price { get; set; }
        [Required]
        public required string Size { get; set; }
        [Required]
        public required string ProductName { get; set; }
    }
}