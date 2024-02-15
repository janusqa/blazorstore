using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace BlazorStore.Models.Domain
{
    public class ProductPrice : BaseEntity
    {
        [Key]
        public int Id { get; set; }
        [Range(1, int.MaxValue)]
        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        [ValidateNever]
        public Product? Product { get; set; }
        [Required]
        public required string Size { get; set; }
        [Required]
        public double Price { get; set; }
    }
}