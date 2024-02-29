using System.ComponentModel.DataAnnotations;
using BlazorStore.Dto;

namespace BlazorStore.Client.ViewModels
{
    public class ProductDetailVm
    {
        [Range(1, int.MaxValue, ErrorMessage = "Please enter a value greater than 0")]
        public int Count { get; set; }
        [Required]
        public int SelectedProductPriceId { get; set; }
        public required ProductPriceDto ProductPrice { get; set; }
    }
}