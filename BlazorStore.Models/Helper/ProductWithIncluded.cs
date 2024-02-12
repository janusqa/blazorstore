using System.ComponentModel.DataAnnotations.Schema;
using BlazorStore.Models.Domain;

namespace BlazorStore.Models.Helper
{
    public class ProductWithIncluded : Product
    {
        public required string CategoryName { get; set; }
        [NotMapped]
        public new Category? Category { get; set; }
    }
}