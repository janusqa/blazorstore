using BlazorStore.Service.IService;
using Microsoft.AspNetCore.Components.Forms;

namespace BlazorStore.Service
{
    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _whe;

        public FileService(IWebHostEnvironment whe)
        {
            _whe = whe;
        }

        public bool DeleteFile(string? existingImageUrl)
        {
            if (existingImageUrl is not null && existingImageUrl != "" && !existingImageUrl.Contains("default.png"))
            {
                string wwwRootPath = _whe.WebRootPath;
                var existingImage = Path.Combine(wwwRootPath, existingImageUrl[1..]);
                if (System.IO.File.Exists(existingImage))
                {
                    System.IO.File.Delete(existingImage);
                    return true;
                }
            }
            return false;
        }

        public async Task<(string? ImageUrl, string? Error)> PostFile(IBrowserFile file, string? existingImageUrl = null)
        {
            string? ImageUrl = null;
            try
            {
                if (file is not null)
                {
                    string wwwRootPath = _whe.WebRootPath;
                    string urlPath = @"images/product";
                    string fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.Name)}";
                    string fileDirectory = Path.Combine(wwwRootPath, urlPath);
                    string filePath = Path.Combine(fileDirectory, fileName);

                    if (!Directory.Exists(fileDirectory))
                    {
                        Directory.CreateDirectory(fileDirectory);
                    }

                    // if a file was uploaded and there is an existing file
                    // we need to repalce the existing file by first deleting 
                    // it and then copying in the new file. Otherwise, just
                    // copy in the new file
                    DeleteFile(existingImageUrl);


                    using (FileStream writer = new(filePath, FileMode.Create))
                    {
                        await file.OpenReadStream().CopyToAsync(writer);
                    }

                    ImageUrl = @$"/{urlPath}/{fileName}";
                }
                else
                {
                    // if no file is uploaded but a file is already in the database
                    // keep that file, otherwise set ImageUrl to empty string.
                    ImageUrl =
                        existingImageUrl is null || existingImageUrl == ""
                        ? ""
                        : existingImageUrl;
                }

                return (ImageUrl, null);
            }
            catch (Exception ex)
            {
                return (ImageUrl, ex.Message);
            }
        }

        public async Task<(string? ImageUrl, string? Error)> PostFileSSR(IFormFile file, string? existingImageUrl = null)
        {
            string? ImageUrl = null;

            try
            {
                if (file is not null)
                {
                    string wwwRootPath = _whe.WebRootPath;
                    string urlPath = @"images/product";
                    string fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                    string fileDirectory = Path.Combine(wwwRootPath, urlPath);
                    string filePath = Path.Combine(fileDirectory, fileName);

                    if (!Directory.Exists(fileDirectory))
                    {
                        Directory.CreateDirectory(fileDirectory);
                    }

                    // if a file was uploaded and there is an existing file
                    // we need to repalce the existing file by first deleting 
                    // it and then copying in the new file. Otherwise, just
                    // copy in the new file
                    DeleteFile(existingImageUrl);

                    using (FileStream writer = new(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(writer);
                    }

                    ImageUrl = @$"/{urlPath}/{fileName}";
                }
                else
                {
                    // if no file is uploaded but a file is already in the database
                    // keep that file, otherwise set ImageUrl to empty string.
                    ImageUrl =
                        existingImageUrl is null || existingImageUrl == ""
                        ? ""
                        : existingImageUrl;
                }
                return (ImageUrl, null);
            }
            catch (Exception ex)
            {
                return (ImageUrl, ex.Message);
            }
        }
    }
}