# Kysymykset — Osa 1: Lokaali kehitys

Vastaa kysymyksiin omin sanoin. Lyhyet, selkeät vastaukset riittävät — tarkoitus on osoittaa, että olet ymmärtänyt konseptit.

---

## Clean Architecture

**1.** Selitä omin sanoin: mitä tarkoittaa, että `UploadPhotoUseCase` "ei tiedä" tallennetaanko kuva paikalliselle levylle vai Azureen? Näytä koodirivit, jotka osoittavat tämän.

> Vastauksesi: Se ei tiedä tallennetaanko kuva levyle vai Azureen, koska se kutsuu vain rajapintaa IStorageService, eli ei konkreettista toteutusta.

---

**2.** Miksi `IStorageService`-rajapinta on määritelty `GalleryApi.Domain`-kerroksessa, mutta `LocalStorageService` on `GalleryApi.Infrastructure`-kerroksessa? Mitä hyötyä tästä jaosta on?

> Vastauksesi: IStorageService on Domainissa; se on pelkkä rajapinta, johon sovelluslogiikka menee ilman teknisiä riippuvuuksia. LocalStorageService on Infrastructuressa, koska se on konkreettinen toteutus, joka voidaan vaihtaa ilman että Domain tai Application muuttuisi.

---

**3.** Testit käyttävät `Mock<IAlbumRepository>`. Mitä mock-objekti tarkoittaa, ja miksi Clean Architecture tekee tämän testaustavan mahdolliseksi?

> Vastauksesi: Mock-objekti on testissä käytettävä feikki, joka matkii oikean rajapinnan toimintaa. Clean Architecture mahdollistaa tämän sikisi, koska käyttötapaukset ei riipu tietokannasta vaan rajapinnoista.

---

## Salaisuuksien hallinta

**4.** Kovakoodattu API-avain on ongelma, vaikka repositorio olisi yksityinen. Selitä kaksi eri syytä miksi.

> Vastauksesi: Kovakoodattu avain voi vuotaa GitHubiin ja päätyä muiden käyttöön. Lisäksi avain voi jäädä repositorion historiaan, vaikka sen poistaisi myöhemmin.

---

**5.** Riittääkö se, että poistat kovakoodatun avaimen myöhemmässä commitissa? Perustele vastauksesi.

> Vastauksesi: Ei riitä, Git tallentaa kaiken historiaan ja siksi avain on edelleen löydettävissä vanhoista commiteista. Turvallisin tapa on kylmästi poistaa avain ja kierrättää se uudeksi.

---

**6.** Minne User Secrets tallennetaan käyttöjärjestelmässä? (Mainitse sekä Windows- että Linux/macOS-polut.) Miksi tämä sijainti on turvallinen?

> Vastauksesi: Sijainti on turvallinen, koska se ei koskaan päädy repoon eikä jaa siten avaimia muille käyttäjille.

---

## Options Pattern ja konfiguraatio

**7.** Mitä hyötyä on `IOptions<ModerationServiceOptions>`:n käyttämisestä verrattuna siihen, että luetaan arvo suoraan `IConfiguration`-rajapinnalta (`configuration["ModerationService:ApiKey"]`)?

> Vastauksesi: Estää kirjoitusvirheet, koska käytetään vahvasti tyypitettyä luokkaa.

---

**8.** ASP.NET Core lukee konfiguraation useista lähteistä prioriteettijärjestyksessä. Listaa lähteet korkeimmasta matalimpaan ja selitä, mikä arvo lopulta käytetään, kun sama avain on sekä `appsettings.json`:ssa että User Secretsissä.

> Vastauksesi: Jos sama avain on sekä appsettings.jsonissa että User Secretsissä, käytetään User Secrets -arvoa.

---

**9.** `DependencyInjection.cs`:ssä valitaan tallennustoteutus näin:

```csharp
var provider = configuration["Storage:Provider"] ?? "local";
if (provider == "azure")
    services.AddScoped<IStorageService, AzureBlobStorageService>();
else
    services.AddScoped<IStorageService, LocalStorageService>();
```

Miksi käytetään konfiguraatioarvoa `env.IsDevelopment()`-tarkistuksen sijaan? Mitä haittaa olisi `if (env.IsDevelopment()) { käytä lokaalia }`-lähestymistavassa?

> Vastauksesi: Konfiguraatioarvo antaa joustavuutta, koska tallennuspaikkaa voi vaihtaa ilman koodimuutoksia.

---

## Tiedostotallennus

**10.** Kun lataat kuvan, `imageUrl`-kentän arvo on `/uploads/abc123-..../photo.jpg`. Miten tähän URL:iin pääsee selaimella? Mihin koodiin tämä perustuu?

> Vastauksesi: Selaimella pääsee URL:iin, koska UseStaticFiles() palvelee wwwroot-kansion sisältöä automaattisesti.

---

**11.** Mitä tapahtuu jos yrität ladata tiedoston jonka MIME-tyyppi on `application/pdf`? Missä tiedostossa ja millä koodirivillä tämä käyttäytyminen on määritelty?

> Vastauksesi: PDF hylätään, koska MIME-tyyppi tarkistetaan ploadPhotoUseCase-luokassa rivillä jossa verrataan sallittuihin tyyppeihin.

---

**12.** `DeletePhotoUseCase` poistaa tiedoston kutsumalla `_storageService.DeleteAsync(photo.FileName, photo.AlbumId)` — ei `photo.ImageUrl`:lla. Miksi?

> Vastauksesi: Azure ja paikallinen tallennus tarvitsevat oikean tiedostonimen, jotta tiedosto voidaan poistaa.
