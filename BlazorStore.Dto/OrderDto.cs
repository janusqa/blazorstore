namespace BlazorStore.Dto
{
    public record OrderDto
    {
        public required OrderHeaderDto OrderHeader { get; set; }
        public required List<OrderDetailDto> OrderDetails { get; set; }
    }
}