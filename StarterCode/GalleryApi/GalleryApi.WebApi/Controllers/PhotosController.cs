using GalleryApi.Application.DTOs;
using GalleryApi.Application.UseCases.Photos;
using GalleryApi.WebApi.Contracts.Photos;
using Microsoft.AspNetCore.Mvc;

namespace GalleryApi.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PhotosController : ControllerBase
{
    private readonly GetPhotosByAlbumUseCase _getPhotosByAlbum;
    private readonly UploadPhotoUseCase _uploadPhoto;

    public PhotosController(
        GetPhotosByAlbumUseCase getPhotosByAlbum,
        UploadPhotoUseCase uploadPhoto)
    {
        _getPhotosByAlbum = getPhotosByAlbum;
        _uploadPhoto = uploadPhoto;
    }

    [HttpGet("{albumId:guid}")]
    public async Task<ActionResult<IEnumerable<PhotoDto>>> GetByAlbum(Guid albumId)
    {
        try
        {
            return Ok(await _getPhotosByAlbum.ExecuteAsync(albumId));
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPost("{albumId:guid}")]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<PhotoDto>> Upload(
        [FromRoute] Guid albumId,
        [FromForm] UploadPhotoForm form)
    {
        if (form.File == null || form.File.Length == 0)
            return BadRequest(new { Error = "Tiedosto puuttuu." });

        await using var stream = form.File.OpenReadStream();

        var request = new UploadPhotoRequest(
            AlbumId: albumId,
            Title: form.Title,
            FileStream: stream,
            FileName: form.File.FileName,
            ContentType: form.File.ContentType,
            FileSize: form.File.Length
        );

        try
        {
            var result = await _uploadPhoto.ExecuteAsync(request);
            if (!result.IsSuccess)
                return BadRequest(new { Error = result.Error });

            return CreatedAtAction(nameof(GetByAlbum), new { albumId }, result.Value);
        }
        catch (NotImplementedException ex)
        {
            return StatusCode(501, new { Error = ex.Message });
        }
    }
}
