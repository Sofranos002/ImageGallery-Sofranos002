using Azure.Identity;
using GalleryApi.Application;
using GalleryApi.Infrastructure;
using GalleryApi.Infrastructure.Moderation;
using GalleryApi.Infrastructure.Options;
using GalleryApi.Infrastructure.Persistence;
using GalleryApi.Infrastructure.Storage;   // UUSI
using GalleryApi.Domain.Interfaces;   // UUSI

var builder = WebApplication.CreateBuilder(args);

// ============================================================
// Key Vault konfiguraatiolähteeksi (Azuressa)
// ============================================================
var keyVaultUrl = builder.Configuration["KeyVault:VaultUrl"];
if (!string.IsNullOrEmpty(keyVaultUrl))
{
    builder.Configuration.AddAzureKeyVault(
        new Uri(keyVaultUrl),
        new DefaultAzureCredential());
}

// ============================================================
// Moderation Options Pattern
// ============================================================
builder.Services.Configure<ModerationServiceOptions>(
    builder.Configuration.GetSection(ModerationServiceOptions.SectionName));

builder.Services.AddSingleton<ModerationServiceClient>();

// ============================================================
// Storage Options Pattern (AzureBlobStorageService tarvitsee tämän)
// ============================================================
builder.Services.Configure<StorageOptions>(                 // UUSI
    builder.Configuration.GetSection("Storage"));           // UUSI

// ============================================================
// Valitse tallennuspalvelu Provider-arvon perusteella
// ============================================================
var storageProvider = builder.Configuration["Storage:Provider"];

if (storageProvider == "azure")                             // UUSI
{
    builder.Services.AddScoped<IStorageService, AzureBlobStorageService>();   // UUSI
}
else
{
    builder.Services.AddScoped<IStorageService, LocalStorageService>();       // UUSI
}

// ============================================================
// Sovellus- ja infrastruktuurikerrokset
// ============================================================
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new()
    {
        Title = "Gallery API",
        Version = "v1",
        Description = "Kuvakirjasto API — Clean Architecture -esimerkki"
    });
});

var app = builder.Build();

// Tietokanta — luodaan automaattisesti jos ei ole olemassa
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<GalleryDbContext>();
    db.Database.EnsureCreated();
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthorization();
app.MapControllers();

app.Run();