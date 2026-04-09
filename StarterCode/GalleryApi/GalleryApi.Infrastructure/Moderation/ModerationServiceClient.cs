using GalleryApi.Infrastructure.Options;                 // UUSI LISÄYS (VAIHE 4)
using Microsoft.Extensions.Options;                      // UUSI LISÄYS (VAIHE 4)

namespace GalleryApi.Infrastructure.Moderation;

/// <summary>
/// Simuloitu sisällöntarkistuspalvelu. Oikeassa sovelluksessa tämä
/// kutsuisi ulkoista AI-pohjaista moderointipalvelua.
/// </summary>
public class ModerationServiceClient
{
    // ============================================================
    // UUSI LISÄYS (VAIHE 4): Options Pattern -kenttä
    // ============================================================
    private readonly ModerationServiceOptions _options;   // UUSI LISÄYS (VAIHE 4)

    // ============================================================
    // UUSI LISÄYS (VAIHE 4): Oikea konstruktori Options Patternille
    // ============================================================
    public ModerationServiceClient(IOptions<ModerationServiceOptions> options) // UUSI LISÄYS
    {
        _options = options.Value;                        // UUSI LISÄYS
    }

    /// <summary>
    /// Tarkistaa onko kuvan sisältö turvallinen.
    /// Simuloitu toteutus — palauttaa aina true.
    /// </summary>
    public Task<bool> IsContentSafeAsync(Stream imageStream, string contentType)
    {
        // Simuloitu tarkistus: oikeassa toteutuksessa käyttäisi _options.ApiKey
        return Task.FromResult(true);
    }

    // ============================================================
    // Tämä metodi lisättiin aiemmin — jätetään ennalleen
    // ============================================================
    public Task<bool> IsImageAllowedAsync(byte[] imageBytes)
    {
        return Task.FromResult(true);
    }
}

