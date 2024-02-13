using Microsoft.AspNetCore.Components.Forms;

namespace BlazorStore.Service.IService
{
    public interface IFileUpload
    {
        Task<string> PostFile(IBrowserFile file, string? existingImageUrl = null);
        bool DeleteFile(string existingImageUrl);
    }
}