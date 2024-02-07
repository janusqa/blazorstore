
using BlazorStore.Models.Domain;

namespace BlazorStore.Components.Demo
{
    public partial class BindProp
    {
        private string? selectedProp = "";
        Product product = new Product
        {
            Id = 1,
            Name = "Rose Candle",
            IsActive = true,
            Price = 10.99,
            ProductProps = new List<ProductProp>
            {
                new ProductProp {Id=1, Key="Color", Value="Black"},
                new ProductProp {Id=2, Key="Flavor", Value="Rose Jasmine"},
                new ProductProp {Id=3, Key="Size", Value="20oz"}
            }
        };

        List<Product> products = new List<Product>();

        protected override void OnInitialized()
        {
            products.Add(new()
            {
                Id = 1,
                Name = "Midnight Blaze",
                IsActive = false,
                ProductProps = new()
                {
                    new ProductProp { Id = 1, Key = "Flavor", Value = "Rose"},
                    new ProductProp { Id = 2, Key = "Size", Value = "20oz"},
                    new ProductProp { Id = 3, Key = "Color", Value="Purple" }
                }
            });

            products.Add(new()
            {
                Id = 2,
                Name = "Blossom Lily",
                IsActive = true,
                ProductProps = new()
                {
                    new ProductProp { Id = 1, Key = "Flavor", Value = "Lily" },
                    new ProductProp { Id = 2, Key = "Size", Value = "18oz" },
                    new ProductProp {Id = 3,Key = "Color",Value = "White"}
                }
            });
        }
    }
}