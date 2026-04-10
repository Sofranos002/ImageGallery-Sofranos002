# Kysymykset — Osa 2: Azure-julkaisu

Vastaa kysymyksiin omin sanoin. Lyhyet, selkeät vastaukset riittävät.

---

## Azure Blob Storage

**1.** Mitä eroa on `LocalStorageService.UploadAsync`:n ja `AzureBlobStorageService.UploadAsync`:n palauttamilla URL-arvoilla? Miksi ne eroavat?

> Vastauksesi: LocalStorageService palauttaa paikallisen tiedostopolun, kun taas AzureBlobStorageService palauttaa julkisen internet-osoitteen blobille. Miksi eroaa: toinen viittaa koneen tiedostojärjestelmään ja toinen Azure-palveluun.

---

**2.** `AzureBlobStorageService` luo `BlobServiceClient`:n käyttäen `DefaultAzureCredential()` eikä yhteysmerkkijonoa. Mitä etua tästä on? Mitä `DefaultAzureCredential` tekee eri ympäristöissä?

> Vastauksesi: DefaultAzureCredential valitsee automaattisesti sopivan kirjautumistavan. Kun ei tartte säilyttää yhteismerkkijonoja, niin se parantaa turvallisuutta ja ylläpidettävyyttä.

---

**3.** Blob Container luodaan `--public-access blob` -asetuksella. Mitä tämä tarkoittaa: mitä pystyy tekemään ilman tunnistautumista, ja mikä vaatii Managed Identityn?

> Vastauksesi: Blobit ovat luettavissa ilman kirjautumista. Kirjoittaminen, poistaminen ja lataaminen vaativat Managed Identityn.

---

## Application Settings

**4.** Application Settings ylikirjoittavat `appsettings.json`:n arvot. Selitä tämä mekanismi: miten se toimii ja miksi se on hyödyllistä eri ympäristöjä varten?

> Vastauksesi: Azure lataa appsettings.json → sitten ylikirjoittaa arvot Application Settings -asetuksilla. Tämä mahdollistaa eri ympäristöjen (dev/test/prod) omat asetukset ilman koodimuutoksia.

---

**5.** Application Settingsissa käytetään `Storage__Provider` (kaksi alaviivaa), mutta koodissa luetaan `configuration["Storage:Provider"]` (kaksoispiste). Miksi?

> Vastauksesi: Azure käyttää kaksoisalaviivaa ympäristömuuttujissa, koska kaksoispistettä ei voi käyttää.

---

**6.** Mitkä konfiguraatioarvot soveltuvat Application Settingsiin, ja mitkä eivät? Anna esimerkki kummastakin tässä tehtävässä.

> Vastauksesi: Soveltuu: mpäristökohtaiset asetukset, kuten Storage Provider tai Azure Storage Accountin nimi. Ei sovellu: salaiset avaimet tai yhteysmerkkijonot.

---

## Managed Identity ja RBAC

**7.** Selitä omin sanoin: mitä tarkoittaa "System-assigned Managed Identity"? Mitä tapahtuu tälle identiteetille, jos App Service poistetaan?

> Vastauksesi: Se on Azure-palvelun automaattisesti luoma identiteetti, jota Azure hallitsee. Jos se poistetaan, myös identiteetti poistuu automatiko.

---

**8.** App Servicelle annettiin `Storage Blob Data Contributor` -rooli Storage Accountin tasolle — ei koko subscriptionin tasolle. Miksi tämä on parempi tapa? Mikä periaate tähän liittyy?

> Vastauksesi: Oikeudet annetaan vain siihen resurssiin, jota tarvitaan → minimoi riskit. Tämä noudattaa Least Privilege -periaatetta.

---


