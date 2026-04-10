using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace GalleryApi.WebApi.Contracts.Photos;

public class UploadPhotoForm
{
    [Required]
    public string Title { get; set; } = string.Empty;

    [Required]
    public IFormFile File { get; set; } = default!;
}