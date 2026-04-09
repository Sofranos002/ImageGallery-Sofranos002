using GalleryApi.Domain.Interfaces;
using GalleryApi.Infrastructure.Options;
using GalleryApi.Infrastructure.Persistence;
using GalleryApi.Infrastructure.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GalleryApi.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Tietokanta (SQLite kehityksessä, voidaan vaihtaa SQL Serveriin tuotannossa)
        services.AddDbContext<GalleryDbContext>(options =>
            options.UseSqlite(
                configuration.GetConnectionString("Default") ?? "Data Source=gallery.db"));

        // Repositoriot
        services.AddScoped<IAlbumRepository, AlbumRepository>();
        services.AddScoped<IPhotoRepository, PhotoRepository>();

        // ============================================================
        // POISTETTU VANHA RIVI (Vaihe 6):
        // services.AddScoped<IStorageService, LocalStorageService>();
        // ============================================================

        // ============================================================
        // UUSI LISÄYS (Vaihe 6): Ehdollinen provider-logiikka
        // ============================================================
        var provider = configuration[$"{StorageOptions.SectionName}:Provider"]
            ?? StorageOptions.LocalProvider;

        if (provider == StorageOptions.AzureProvider)
        {
            // HUOM: AzureBlobStorageService ei ole toteutettu tässä harjoituksessa
            // services.AddScoped<IStorageService, AzureBlobStorageService>();
        }
        else
        {
            services.AddScoped<IStorageService, LocalStorageService>(); // Osa 1 — tämä nyt
        }

        return services;
    }
}
