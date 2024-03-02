using System.ComponentModel.DataAnnotations.Schema;
using BlazorStore.Models.Domain;

namespace BlazorStore.Models.Helper
{
    public class ProductWithCategory : Product
    {
        public required string CategoryName { get; set; }
        [NotMapped]
        public new Category? Category { get; set; }
    }

    public class ProductWithPrice : ProductWithCategory
    {
        public int? ProductPriceId { get; set; }
        public string? Size { get; set; }
        public double? Price { get; set; }

    }
}