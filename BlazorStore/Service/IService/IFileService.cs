using Microsoft.AspNetCore.Components.Forms;

namespace BlazorStore.Service.IService
{
    public interface IFileService
    {
        Task<string> PostFile(IBrowserFile file, string? existingImageUrl = null);
        Task<string> PostFileSSR(IFormFile Image, string? existingImageUrl = null);
        bool DeleteFile(string? existingImageUrl);
    }
}