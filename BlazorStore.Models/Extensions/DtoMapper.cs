
using BlazorStore.Dto;
using BlazorStore.Models.Domain;

namespace BlazorStore.Models.Extensions
{
    public static class DtoMapper
    {
        // NB this is an "extension method" for model
        // the "this" keyword allows this to appear as a member method
        // of the model. It allows us to call it like myModel.ToDto
        // which looks much better than DomainExtension.ToDto(myModel).
        // aka it is syntactic sugar over the static method.
        public static CategoryDto ToDto(this Category category)
        {
            return new CategoryDto(
                Id: category.Id,
                Name: category.Name
            );
        }

        public static UpdateCategoryDto ToUpdateDto(this Category category)
        {
            return new UpdateCategoryDto(
                Id: category.Id,
                Name: category.Name

            );
        }
    }
}