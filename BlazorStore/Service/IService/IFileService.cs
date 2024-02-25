using Microsoft.AspNetCore.Components.Forms;

namespace BlazorStore.Service.IService
{
    public interface IFileService
    {
        Task<(string? ImageUrl, string? Error)> PostFile(IBrowserFile file, string? existingImageUrl = null);
        Task<(string? ImageUrl, string? Error)> PostFileSSR(IFormFile Image, string? existingImageUrl = null);
        bool DeleteFile(string? existingImageUrl);
    }
}