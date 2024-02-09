using BlazorStore.Models.Domain;

namespace BlazorStore.Components.Pages.Demo
{
    public partial class ProductPage
    {
        private List<Product> products = new List<Product>();
        private int Favorited { get; set; } = 0;
        private string LastSelctedProduct { get; set; } = "";

        protected override void OnInitialized()
        {
            products.Add(new()
            {
                Id = 1,
                Name = "Midnight Blaze",
                IsActive = false,
                Price = 10.99,
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
                Price = 10.99,
                ProductProps = new()
                {
                new ProductProp { Id = 1, Key = "Flavor", Value = "Lily" },
                new ProductProp { Id = 2, Key = "Size", Value = "18oz" },
                new ProductProp {Id = 3,Key = "Color",Value = "White"}
                }
            });
        }

        protected void UpdateFavorited(bool isSelected)
        {
            if (isSelected)
            {
                Favorited++;
            }
            else
            {
                Favorited--;
            }
        }

        protected void UpdateLastSelectedProduct(string selectedProduct)
        {
            LastSelctedProduct = selectedProduct;

        }
    }
}