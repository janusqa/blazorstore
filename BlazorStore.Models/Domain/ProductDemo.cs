namespace BlazorStore.Models.Domain
{
    public class ProductDemo
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public double Price { get; set; }
        public bool IsActive { get; set; }
        public List<ProductProp>? ProductProps { get; set; }
    }
}
