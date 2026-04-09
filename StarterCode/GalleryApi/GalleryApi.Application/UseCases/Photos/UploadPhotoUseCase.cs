using GalleryApi.Application.Common;
using GalleryApi.Application.DTOs;
using GalleryApi.Domain.Entities;
using GalleryApi.Domain.Interfaces;

namespace GalleryApi.Application.UseCases.Photos;

public class UploadPhotoUseCase
{
    private readonly IPhotoRepository _photoRepository;
    private readonly IAlbumRepository _albumRepository;
    private readonly IStorageService _storageService;

    private static readonly string[] AllowedContentTypes =
        ["image/jpeg", "image/png", "image/webp", "image/gif"];

    private const long MaxFileSizeBytes = 10 * 1024 * 1024;

    public UploadPhotoUseCase(
        IPhotoRepository photoRepository,
        IAlbumRepository albumRepository,
        IStorageService storageService)
    {
        _photoRepository = photoRepository;
        _albumRepository = albumRepository;
        _storageService = storageService;
    }

    public async Task<Result<PhotoDto>> ExecuteAsync(UploadPhotoRequest request)
    {
        // ============================================================
        // 1. Tarkista että albumi on olemassa
        // ============================================================
        var album = await _albumRepository.GetByIdAsync(request.AlbumId);   // UUSI LISÄYS (VAIHE 7)
        if (album is null)                                                 // UUSI LISÄYS
            return Result<PhotoDto>.Failure($"Albumia {request.AlbumId} ei löydy."); // UUSI LISÄYS

        // ============================================================
        // 2. Validoi tiedostotyyppi
        // ============================================================
        if (!AllowedContentTypes.Contains(request.ContentType))            // UUSI LISÄYS
        {
            return Result<PhotoDto>.Failure(                               // UUSI LISÄYS
                $"Tiedostotyyppi '{request.ContentType}' ei ole sallittu. " +
                $"Sallitut tyypit: {string.Join(", ", AllowedContentTypes)}");
        }

        // ============================================================
        // 3. Validoi tiedoston koko
        // ============================================================
        if (request.FileSize > MaxFileSizeBytes)                           // UUSI LISÄYS
        {
            return Result<PhotoDto>.Failure(                               // UUSI LISÄYS
                $"Tiedosto on liian suuri. Maksimikoko on {MaxFileSizeBytes / (1024 * 1024)} MB.");
        }

        // ============================================================
        // 4. Lataa tiedosto tallennuspalveluun (try-catch)
        // ============================================================
        string imageUrl;                                                   // UUSI LISÄYS
        try
        {
            imageUrl = await _storageService.UploadAsync(                  // UUSI LISÄYS
                request.FileStream,
                request.FileName,
                request.ContentType,
                request.AlbumId);
        }
        catch (Exception ex)
        {
            return Result<PhotoDto>.Failure(                               // UUSI LISÄYS
                $"Tiedoston tallennus epäonnistui: {ex.Message}");
        }

        // ============================================================
        // 5. Luo Photo-entiteetti ja tallenna tietokantaan
        // ============================================================
        var photo = new Photo                                              // UUSI LISÄYS
        {
            Id = Guid.NewGuid(),
            AlbumId = request.AlbumId,
            Title = request.Title,
            FileName = request.FileName,
            ImageUrl = imageUrl,
            ContentType = request.ContentType,
            FileSizeBytes = request.FileSize,
            UploadedAt = DateTime.UtcNow
        };

        var saved = await _photoRepository.CreateAsync(photo);             // UUSI LISÄYS

        // ============================================================
        // 6. Palauta DTO
        // ============================================================
        return Result<PhotoDto>.Success(                                   // UUSI LISÄYS
            new PhotoDto(
                saved.Id,
                saved.AlbumId,
                saved.Title,
                saved.ImageUrl,
                saved.ContentType,
                saved.FileSizeBytes,
                saved.UploadedAt));
    }
}