using System.ComponentModel.DataAnnotations;

namespace BlazorStore.Dto
{
    public record OrderHeaderDto
    {
        public int Id { get; set; }
        [Required]
        public required string UserId { get; set; }
        [Required]
        [EmailAddress]
        public required string Email { get; set; }
        [Required]
        [Display(Name = "Order Total")]
        public double OrderTotal { get; set; }
        [Required]
        [Display(Name = "Order Date")]
        public DateTime OrderDate { get; set; }
        [Required]
        [Display(Name = "Shipping Date")]
        public DateTime ShippingDate { get; set; }
        [Required]
        public required string Status { get; set; }
        public string? SessionId { get; set; }
        public string? PaymentIntentId { get; set; }
        public string? Tracking { get; set; }
        public string? Carrier { get; set; }
        [Required]
        public required string Name { get; set; }
        [Required]
        [Display(Name = "Phone Number")]
        public required string PhoneNumber { get; set; }
        [Required]
        [Display(Name = "Street Address")]
        public required string StreetAddress { get; set; }
        [Required]
        public required string State { get; set; }
        [Required]
        public required string City { get; set; }
        [Required]
        [Display(Name = "Postal Code")]
        public required string PostalCode { get; set; }
    }
}