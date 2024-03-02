using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BlazorStore.Models.Domain;

namespace BlazorStore.Models.Helper
{
    public class OrderDetailWithHeader : OrderHeader
    {
        [Required]
        public int OrderDetailId { get; set; }
        [Required]
        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        [Required]
        public double Price { get; set; }
        [Required]
        public required string Size { get; set; }
        [Required]
        public int Count { get; set; }
        [Required]
        public required string ProductName { get; set; }
    }


}