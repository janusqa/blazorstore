
namespace BlazorStore.Models.Domain
{
    public interface IBaseEntity
    {
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}