using GalleryApi.Application;
using GalleryApi.Infrastructure;
using GalleryApi.Infrastructure.Moderation;   // ← UUSI using
using GalleryApi.Infrastructure.Options;
using GalleryApi.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

// ============================================================
// ONGELMA: API-avain on kovakoodattu suoraan lähdekoodiin!
// ============================================================
//var moderationClient = new ModerationServiceClient("sk-moderation-hardcoded-dev-12345");
//builder.Services.AddSingleton(moderationClient); // VANHA, poistettu

// ❌ POISTA NÄMÄ (väärä Options-luokka ja väärä osion nimi)
//builder.Services.Configure<ModerationOptions>(
//    builder.Configuration.GetSection("Moderation"));
//
//var moderationOptions = builder.Configuration.GetSection("Moderation").Get<ModerationOptions>();
//builder.Services.AddSingleton(new ModerationServiceClient(moderationOptions.ApiKey));

// ============================================================
// UUSI LISÄYS (VAIHE 4): Options Pattern ModerationServiceOptions
// ============================================================
builder.Services.Configure<ModerationServiceOptions>(                 // UUSI LISÄYS
    builder.Configuration.GetSection(ModerationServiceOptions.SectionName)); // UUSI LISÄYS

// ModerationServiceClient saa asetukset DI:n kautta
builder.Services.AddSingleton<ModerationServiceClient>();            // UUSI LISÄYS


// Konfiguraatio-osiot (Options Pattern)
builder.Services.Configure<StorageOptions>(
    builder.Configuration.GetSection(StorageOptions.SectionName));

// Sovellus- ja infrastruktuurikerrokset
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

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Staattisten tiedostojen jako — tarjoilee wwwroot-kansion sisällön
app.UseStaticFiles();

app.UseAuthorization();
app.MapControllers();

app.Run();