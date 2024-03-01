using System.ComponentModel.DataAnnotations;

namespace BlazorStore.Models.Domain
{
    public class OrderHeader : BaseEntity
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public required string UserId { get; set; }
        // TODO: add navigation propert for user
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