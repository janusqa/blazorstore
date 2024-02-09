
namespace BlazorStore.Models.Domain
{
    public class BaseEntity : IBaseEntity
    {
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}