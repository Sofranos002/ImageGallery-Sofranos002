# Kysymykset — Osa 3: Key Vault ja Infrastructure as Code

Vastaa kysymyksiin omin sanoin. Lyhyet, selkeät vastaukset riittävät.

---

## Key Vault

**1.** Miksi `ModerationService:ApiKey` tallennettiin Key Vaultiin eikä Application Settingsiin? Mitä lisäarvoa Key Vault tuo Application Settingsiin verrattuna?

> Vastauksesi: Sine tallentaminen on turvallisempaa, sillä salaisuudet ei näy suoraa sovelluksen asetuksissa. Hallitaan keskitetysti. Lisäarvo: parempi tietoturva auditoinnin ja pääsynhallinnan kautta.

---

**2.** Key Vault -salaisuuden nimi on `ModerationService--ApiKey` (kaksi väliviivaa), mutta koodissa se luetaan `configuration["ModerationService:ApiKey"]` (kaksoispiste). Miksi käytetään `--`?

> Vastauksesi: Se ei salli kaksoispisteitä nimissä, siksi korvataan kahdella _ _. Asp.net muuntaa sen takaisn kaksoispisteeksi.

---

**3.** `Program.cs`:ssä Key Vault lisätään konfiguraatiolähteeksi `if (!string.IsNullOrEmpty(keyVaultUrl))`-ehdolla. Miksi tämä ehto on tärkeä? Mitä tapahtuisi ilman sitä?

> Vastauksesi: Ilman sitä soevellus yrittää käynnistyä Key Vaultin puuttuessa, ja kaatuisi tästä syystä käynnistymisessä. Eli ehto estää virheen.

---

**4.** Kun sovellus on käynnissä Azuressa, konfiguraation prioriteettijärjestys on: Key Vault → Application Settings → `appsettings.json`. Selitä millä arvolla `ModerationService:ApiKey` lopulta ladataan — ja käy läpi jokainen askel siitä, miten arvo päätyy sovelluksen `IOptions<ModerationServiceOptions>`:iin.

> Vastauksesi: Sovellus hakee ensin arvon Key Vaultista. Jos löytyy, se ylikirjoittaa Application Settingsin ja appsettings.json:n arvot. Lopuksi arvo sidotaan olioon konfiguraatiobindauksen kautta.

---

**5.** Mitä eroa on `Key Vault Secrets User` ja `Key Vault Secrets Officer` -roolien välillä? Miksi annettiin nimenomaan `Secrets User`?

> Vastauksesi: Secrets useri lukee salaisuuksia ja Secrets Officeri pystyy myös luomaan ja muokkaamaan niitä. KOska sovellus tarvitsee vain lukuoikeuden, annetaan siksi Secret User.

---

## Infrastructure as Code (Bicep)

**6.** Bicep-templatessa RBAC-roolimääritykset tehdään suoraan (`storageBlobRole`, `keyVaultSecretsRole`). Mitä etua tällä on verrattuna siihen, että ajat erilliset `az role assignment create` -komennot käsin?

> Vastauksesi: määritys Bicepissa tekee deploysta toistettvan ja automaattisen ilman manu vaiheita. Virheet vähenee, ja varmistettaan että oikeudet on oikein asetettu.

---

**7.** Bicep-parametritiedostossa `main.bicepparam` on `param moderationApiKey = ''` — arvo jätetään tyhjäksi. Miksi? Miten oikea arvo annetaan?

> Vastauksesi: Koska sitä ei kovakoodata tiedostoon turvallisuus syystä. OIkean arvon voi antaa deploy vaiheessa esim. pipeline muuttujan kautta.

---

**8.** Bicep-templatessa `webApp`-resurssin `identity`-lohkossa on `type: 'SystemAssigned'`. Mitä tämä tekee, ja mitä manuaalista komentoa se korvaa?

> Vastauksesi: 

---

**9.** RBAC-roolimäärityksen nimi generoidaan `guid()`-funktiolla:

```bicep
name: guid(storageAccount.id, webApp.identity.principalId, 'StorageBlobDataContributor')
```

Miksi nimi generoidaan näin eikä esimerkiksi kovakoodatulla merkkijonolla? Mitä tapahtuisi jos nimi olisi sama kaikissa deploymenteissa?

> Vastauksesi:guidi varmistaa, että roolin määritys on uniikki tiettyjen arvojen perusteella. JOs sama nimi esityisi kaikissa deployissa, niin syntyisi konflikteja ja päällekäisiä määrityksiä.

---

**10.** Olet nyt rakentanut saman infrastruktuurin kahdella tavalla: manuaalisesti (Osat 2 & 3) ja Bicepillä (Osa 3). Kuvaile konkreettisesti yksi tilanne, jossa IaC-lähestymistapa on selvästi manuaalista parempi. Kuvaile myös tilanne, jossa manuaalinen tapa riittää.

> Vastauksesi: LaCi on parepmpi esim silloin, kun ympäristö luodaan useamman kerran identtisenä. Manu tapa riittää pienemmissä ja kertaluontoisemmissa kokeiluissa.
