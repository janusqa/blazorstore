using System.ComponentModel.DataAnnotations;

namespace BlazorStore.Models.Domain
{
    public class Category : BaseEntity
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(30)]
        public required string Name { get; set; }
    }
}