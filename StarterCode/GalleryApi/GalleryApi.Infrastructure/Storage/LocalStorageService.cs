using GalleryApi.Domain.Interfaces;
using GalleryApi.Infrastructure.Options;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;

namespace GalleryApi.Infrastructure.Storage;

public class LocalStorageService : IStorageService
{
    private readonly string _basePath;   // OK

    // POISTA (VAIHE 5): _baseUrl ei kuulu Vaiheeseen 5
    // private readonly string _baseUrl;

    public LocalStorageService(IWebHostEnvironment env, IOptions<StorageOptions> opts)
    {
        _basePath = Path.Combine(env.ContentRootPath, opts.Value.BasePath);

        // POISTA (VAIHE 5): URL-laskenta ei kuulu Vaiheeseen 5
        // _baseUrl = opts.Value.BaseUrl;
    }

    // UUSI LISÄYS (VAIHE 5): async + oikea URL + CopyToAsync
    public async Task<string> UploadAsync(Stream fileStream, string fileName, string contentType, Guid albumId)
    {
        var albumDir = Path.Combine(_basePath, albumId.ToString());
        Directory.CreateDirectory(albumDir);

        var filePath = Path.Combine(albumDir, fileName);

        using var output = File.Create(filePath);
        await fileStream.CopyToAsync(output);   // UUSI LISÄYS (VAIHE 5)

        // UUSI LISÄYS (VAIHE 5): kovakoodattu URL ohjeen mukaan
        return $"/uploads/{albumId}/{fileName}";
    }

    public Task DeleteAsync(string fileName, Guid albumId)
    {
        var filePath = Path.Combine(_basePath, albumId.ToString(), fileName);

        if (File.Exists(filePath))
            File.Delete(filePath);

        return Task.CompletedTask;
    }
}