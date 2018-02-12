# C# ja ASP.NET Core 2.0

ASP.NET Core on Microsoftin tekemä web-palvelinohjelmointiin tarkoitettu ohjelmistokehys. Ensimmäinen versio tuli 2016, ja uusin versio tällä hetkellä on 2.0 joka on julkaistu 2017 elokuussa. ASP.NETistä on vanhempi versio jota on kehitetty vuodesta 2002 mutta ASP.NET Core on kokonaan uudelleenkirjoitettu versio joka ei ole taaksepäin yhteensopiva.

Samalla Microsoft julkaisi .NET Core ohljelmointiympäristön ja työkalut jotka toimivat useassa käyttöjärjestelmässä toisin kuin vanhemmat C# .NET Framework työkalut. .NET Corelle suunnattua ohjelmaa voidaan ajaa esimerkiksi pienissä Linux virtuaalikoneissa, macOS:llä tai Windowsilla.

ASP.NET Core sisältää useita kirjastoja, joista käsittelen tärkeimpiä:

* MVC - Pääkirjasto jolla voidaan toteuttaa mm. REST rajapinnat, sisältää useita kirjastoja sisällään.
* Entity Framework Core - Microsoftin tekemä tietokantakirjasto ja ORM.
* Identity Core - Kirjautumisjärjestelmän ja käyttäjien hallintaa.

Asennan myös ohjelmaan [Swagger REST-rajapintatesterin](https://swagger.io/).

Seuraavissa kohdissa esimerkkinä rakennettu ohjelma toimii kaikilla .NET Core ympäristöillä.

## Asennus ja perusteita

Asenna ensin .NET Core omalle koneelle: [Windows](https://www.microsoft.com/net/learn/get-started/windows), [Linux](https://www.microsoft.com/net/learn/get-started/linuxredhat) tai [MacOS](https://www.microsoft.com/net/learn/get-started/macos)

Siirry tyhjään hakemistoon, huomaa että hakemiston nimestä tulee esimerkkiohjelman nimiavaruus. Aja hakemistossa seuraava komento:

```bash
dotnet new webapi
```

Koeta että ohjelma käynnistyy ja toimii oikein:

```bash
dotnet run
```

Voit nyt navigoida selaimella suoraan osoitteeseen: [http://localhost:5000/api/Values/](http://localhost:5000/api/Values/) jonka pitäisi tulostaa JSON `["value1","value2"]`.

`dotnet` komento on luonut pohjan ja esimerkin hyvin pienelle REST API:lle. Esimerkki on luonut seuraavankaltaisen hakemistorakenteen:

```
Esimerkki1
│
├── appsettings.Development.json    // Projektisi (kehityksen aikaisia) asetuksia
├── appsettings.json                // Projektisi omia asetuksia, näitä voi luoda itse
├── Controllers                     // 
│   └── ValuesController.cs         // REST rajapinnan esimerkkicontroller
├── Esimerkki1.csproj               // Projektitiedosto
├── obj                             // Käännöksen aikasia artefakteja
│   └── ...
├── Program.cs                      // Projektisi komentorivin Main
├── Startup.cs                      // Projektisi Servicet
└── wwwroot                         // Staattisten tiedostojen hakemisto (tyhjä)
    └── ...
```

### appsettings.*.json

Nämä tiedostot ovat ohjelmasi konfiguraatioita varten, ja sinne voit luoda omia asetuksia joita ohjelmasi tarvitsee. Jos ohjelmakoodisi ei tarvitse konfiguraatioita, voit vaikka poistaa tiedostot. 

Näiden sisältö on siis täysin ohjelmasi konfigurointia varten johon luodaan itse asetuksia, vaikka siellä on Debug asetuksia perusesimerkkinä.

Yleensä näihin tiedostoihin tulee ohjelman ajonaikaisia asetuksia esimerkiksi tietokantayhteyteen liittyvät asetukset, sähköpostipalvelimen asetukset, ulkoisia rajapintaosoitteita, esim maksurajapinnan osoitteet, ym.

### Startup.cs

Tämä on ASP.NET Coren tärkein konfigurointitiedosto, joka on ASP.NET Corelle ominainen. 

#### `Configuration` property
Tästä objektista voi hakea asetuksia projektisi `appsettings.*.json`. tiedostoista, asetukset tulevat myös ympäristömuuttujasta (Environment).

#### `void ConfigureServices(IServiceCollection services)` metodi
Tässä metodissa konfiguroidaan ohjelmasi servicet, eli dependency injection riippuvuudet. Tästä tarkemmin alapuolella.

#### `void Configure(IApplicationBuilder app, IHostingEnvironment env)` metodi
Täällä konfiguroidaan ohjelmasi HTTP pipeline, esimerkiksi middlewaret, jotka liittyvät HTTP Requestin kulkuun järjestelmän lävitse.

Asentamasi lisäkirjastot yleensä konfiguroidaan jommassa kummassa metodissa, yleinen konventio on että lisäkirjastot konfiguroidaan extensio metodeilla. `Configure`-metodissa lisäkirjastojen extensiot ovat nimetty `app.UseJokinToinenKirjasto()` ja `ConfigureServices`-metodissa `services.AddJokinToinenKirjasto()`.

Lisäkirjastojen konfiguroinnissa Use ja Add lausekkeiden järjestyksellä on tärkeä merkitys, middlewaret rekisteröityvät antamassasi järjestyksessä. Esimerkiksi autentikointiin liittyvät saattaa olla pakollista rekisteröidä ensin jotta ne toimivat oikein.


### Esimerkk1.csproj

Projektitiedosto, jossa riippuvuudet ja kääntämiseen liittyviä asetuksia on määritelty. Microsoft kehitti uuden, pienemmän projektitiedosto formaatin aikasemman tilalle, tämä on suhteellisen helposti käsin hallittavissa oleva xml tiedosto. Visual Studio osaa suurimmalti osin tiedoston hallinnan graafisesti, mutta sitä joutuu vielä käsittelemään usein suoraan tekstieditorilla.

ASP.NET Core 2:ssa kaikki riippuvuudet ovat metapaketissa `Microsoft.AspNetCore.All`. Vaikka riippuvuus on sidottu pakettiin joka viittaa kaikkiin kirjastoihin, ylimääräiset riippuvuudet [poistetaan automaattisesti](https://andrewlock.net/the-microsoft-aspnetcore-all-metapackage-is-huge-and-thats-awesome-thanks-to-the-net-core-runtime-store-2/) käännön yhteydessä.

### dotnet watch

Tarkoituksena on kääntää ja käynnistää ohjelma automaattisesti uudestaan kun ohjelmakoodia muokataan, tämä nopeuttaa työtäsi kun muokkaat ohjelmaa.

Projektitiedostoon (csproj) pitää lisätä `watch` työkalun asentamista varten yksi xml elementti:

```xml
<ItemGroup>
  <DotNetCliToolReference Include="Microsoft.DotNet.Watcher.Tools" Version="2.0.0" />
</ItemGroup>
```

Aja tämän jälkeen `restore` komento joka hakee ripppuvuudet uudestaan projektitiedostosta:
```bash
dotnet restore
``` 

Tämän jälkeen ohjelman voi käynnistää `watch` työkalulla:

```bash
dotnet watch run
```

Watch työkalulle annetaan parametrina toiminto esimerkiksi `run` joka käynnistää ja kääntää ohjelman automaattisesti kun ohjelmakoodia muokataan. Samoin watch työkalulla voi myös ajaa testit automaattisesti esimerkiksi `dotnet watch test`.

### Visual Studio Code

Jos ei halua tai pysty asentamaan Visual Studioa, suosittelen käyttämään [Visual Studio Code editoria](https://code.visualstudio.com/), tähän löytyy [C# lisäosa](https://marketplace.visualstudio.com/items?itemName=ms-vscode.csharp) joka osaa hyödyllisimmät temput kuten `Ctrl+.` joka osaa etsiä kursorin alla olevan riippuvuuden ja syöttämään `using` lauseen automaattisesti tiedostoon.

## Rajapinnan määrittely Routing, Controller ja action

Rajapinta määritellään kontrolleriluokilla, yleensä luokat ovat muotoa esimerkiksi `ValuesController` jossa `Values` saattaa viitata osaan rajapinnan HTTP osoitetta, mutta se ei ole välttämätöntä. Actionilla viitataan HTTP kyselyn käsittelevään metodiin kyseisessä luokassa.

Rajapinnan osoitteen alkuosan muodosta `Route` attribuutti, ja loppuosan actionin määrittelevä `HttpGet` tai esim `HttpPost`, sisään tuleva arvo merkataan attribuutilla `FromQuery` tai `FromBody` esimerkiksi voit korvata **ValuesController.cs** tiedoston kontrollerin seuraavalla:

```cs
// Tämä määrittelee alkuosaksi "api/values"
[Route("api/[controller]")]
public class ValuesController : Controller
{
    // Tämän actionin osoite on: api/values
    // Http verbi GET
    // Palautusmuoto JSON lista stringejä
    [HttpGet]
    public IEnumerable<string> Get()
    {
        return new string[] { "value1", "value2" };
    }

    // Tämän actionin osoite: api/values/TyyppiTurvallinenPalautusMuoto2
    // HTTP verbi: GET
    // maaginen tekstinpätkä [action] korvataan siis metodin nimellä
    // palautetaan JSON objekti
    [HttpGet("[action]")] 
    public EsimerkkiLuokka TyyppiTurvallinenPalautusMuoto()
    {
        return new EsimerkkiLuokka() {
            Id = 123,
            Jotain = "arvo"
        };
    }
    
    // Tämän actionin osoite: api/values/TyyppiTurvallinenSisaantuloMuoto
    // HTTP verbi: POST
    // Sisään tuleva arvo luetaan kyselyn bodystä
    // Palautetaan JSON tekstiarvo 
    [HttpPost("[action]")]
    public string TyyppiTurvallinenSisaantuloMuoto(
        [FromBody] EsimerkkiLuokka value)
    {
        return "ok";
    }

    // Tämän actionin osoite on: api/values/5
    // HTTP verbi: DELETE
    // Sisään tuleva arvo luetaan osana osoitetta
    // Ulos tulee tekstinpätkä JSON:ia
    [HttpDelete("{id}")] 
    public string Tuhooja(int id)
    {
        return "tekstinpätkä";
    }

    // Tämän actionin osoite: api/values/esimerkkiquery?id=5
    // HTTP verbi: GET
    // Sisään tuleva arvo luetaan query parametrinä osoitteesta
    // palautetaan anonyymi objekti
    [HttpGet("esimerkkiquery")] 
    public object Toka([FromQuery] int id)
    {
        // object palautustyyppiä ei suositella
        return new {
            laiskurin = "tapa palauttaa jsonia",
            arvo = id
        };
    }

    public class EsimerkkiLuokka {
        public int Id { get; set; }
        public string Jotain { get; set; }
    }
    
    
    // Tämän actionin osoite: api/values/esimerkkibody
    // HTTP verbi: POST
    // Sisään tuleva arvo luetaan kyselyn bodystä esimerkiksi 
    // JSON muodossa oleva string
    // Palautetaan JSON tekstiarvo 
    [HttpPost("esimerkkibody")]
    public IActionResult Post([FromBody] string value)
    {
        return new JsonResult("jotain tekstiä");
    }
}
```

Route attribuutissa on muutamia maagisia muuttujia, kuten `[controller]` ja `[action]` jotka viittaavat kontrollerin nimeen ja kyseisen actionin nimeen. Samoin sisään tulevia parametrejä voi ripotella `{muuttuja}` tai valinnaiset muuttujat `{muuttuja?}` merkinnällä.

Luokan ei tarvitse periytyä `Controller` luokasta, esimerkiksi jos haluat tehdä protokolla agnostisen API:n, se on mahdollista ilman perintää. Esimerkkinä voisi toteuttaa kontrollerit niin että niiden actionit voisi toimia HTTP:n lisäksi websocketin kautta.

Rajapinta osaa palauttaa ja ottaa sisäänsä suoraan dataa JSON muodossa, mutta kannattaa pysytellä tyyppiturvallisissa palautus ja sisäänotto muodoissa, eli helposti serialisoitavissa perusluokissa.

[Lisätietoja: Routing to Controller Actions](https://docs.microsoft.com/en-us/aspnet/core/mvc/controllers/routing)

## Rajapintatesteri Swagger

ASP.NET Corelle on hyvä rajapintatesteri, jolle on valmiina testerin generoiva paketti, asenna Swaggerin riippuvuus komentoriviltä:

```bash
dotnet add . package Swashbuckle.AspNetCore
```

Nyt jos tarkastelet projektitiedostoa (csproj) näet että sinne on tullut uusi rivi:

```
<PackageReference Include="Swashbuckle.AspNetCore" Version="1.1.0" />
```

Jos lisäät riippuvuuden käsin, joka on mahdollista, pitää aina pakettien lisäämisen jälkeen ajaa: `dotnet restore` joka tarkistaa ja hakee projektitiedostossa määritellyt paketit.

Swagger täytyy syöttää **Startup.cs** tiedostoon, syötä ensin pakettiriippuvuus yläosaan:

```cs
using Swashbuckle.AspNetCore.Swagger;
```

`ConfigureServices` metodiin dokumentoinnin generoiva service:

```cs
services.AddMvc(); // Tämä rivi on jo, mutta tämän jälkeen:

services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Info { Title = "My API", Version = "v1" });
});
```

Sekä `Configure` metodiin swaggerin käyttöliittymän tarjoileva middleware:

```cs
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/swagger.json", "API");
});
```

Käynnistä ohjelma ja avaa osoite [http://localhost:5000/swagger/](http://localhost:5000/swagger/), nyt selaimesta pitäisi aueta seuraavanlainen sivu:

![Swagger v1 api](swagger-values.png)

Kannattaa testailla esimerkkejä, huomattavaa on että vain `EsimerkkiLuokka` objektin palauttava actioni on tyyppiturvallinen tapa palauttaa JSON objekteja. Kun rakentaa omaa API:a kannatta siis kaikkien actionien palautusarvona olla hyvin määritelty luokka, dokumentaation generoinnin pohjalla oleva ApiExplorer kirjasto ei osaa tyypittää `IActionResult` palauttavia metodeja eikä anonyymejä objekteja.

Tämän huomaa myös selaamalla Swaggerin tuottamaa testeriä, vain rajapinnan endpointit `TyyppiTurvallinenSisaantuloMuoto`, ja `TyyppiTurvallinenPalautusMuoto` modeleista on tarkka tyyppi tiedossa. Vaikka Swagger listaa `EsimerkkiLuokka` nimen, se on epäoleellinen tieto, koska JSON struktuurisesti tyypitetty joten vain kentät on tärkeitä.

## Tietokannan lisääminen EntityFrameworkCore tietokantakirjasto

Tietokantaa varten käytetään usein Microsoftin tekemää EntityFrameworkCore kirjastoa joka luo tietokannan luokkien pohjalta. Ensin luodaan yleensä hakemisto ja namespace `Models` jonne tietokannan malli rakennetaan.

Toteutan seuraavana pienen usean käyttäjän laskujen ylläpitojärjestelmän jolla voi lisätä, poistaa, ja muokata laskuja sekä asiakkaita.  Tietokantamallini on seuraava:

![Tietokanta models](tietokanta-models.png)

* ApplicationUser on ohjelman käyttäjät, tämä on yleensä nimetty näin ASP.NET tutoriaaleissa, se periytyy IdentityUser luokasta jossa on valmiiksi tiettyjä propertyjä kuten Email, Username, Password, jne.
* Business on käyttäjän ylläpitämä yritys
* Client on käyttäjän lista asiakkaista
* Invoice on käyttäjän tekemä lasku
* InvoiceRow on laskurivi
* Email on taulu sähköposteja varten

```
Models.cs tiedoston sisältö
```


Riippuvuus suhteet 

```text
Store -> Service -> Controller
```

## MVC pääkirjasto

Middlewaret, Filtterit

## Dependency Injection ohjelmakirjaston ymmärtäminen

ASP.NET Core käyttää Microsoftin tekemää Dependency Injection -kirjastoa hallitsemaan eri osien riippuvuuksia. Tarkoituksena on kirjoittaa riippuvuudet Serviceinä, ja järjestelmä hoitaa näiden luomisen automaattisesti. Samaan tapaan kuin esim. Javassa Google Guice tai PHP:ssä Laravel Service container.

Injektiolla voidaan helpottaa mm. testausta, ja mahdollistaa toteutuksen vaihtamisen suhteellisen helposti niissä osissa missä se on tarpeen.

Riippuvuudet rekisteröidään seuraavalla kolmella tavalla **Startup.cs** tiedoston `ConfigureServices` metodissa:

* `services.AddTransient` - Riippuvuudet jotka ovat elossa mahdollisimman lyhyen ajan, eli ne luodaan kullekkin kohteelle uudelleen.
* `services.AddScoped` - Riippuvuus joka on elossa yhden HTTP kyselyn ajan. Scopeja voi luoda myös muitakin, mutta ASP.NET Coressa yleensä kyse on HTTP-kyselyn ajan elossa olevista olioista.
* `services.AddSingleton` - Riippuvuus joka on elossa koko ohjelman suorituksen ajan.

Kukin käsky ottaa sisäänsä rajapintaluokan tai interfacen, sekä toteutusluokan. Esimerkkinä rekisteröidään interfacelle toteutusluokka:

```cs
services.AddTransient<IEmailSender, EmailSender>()
```

Rajapintaluokka ja toteutusluokka voivat olla samoja, erityisesti kun järjestelmää vasta kehtitetään on yleistä luoda vain toteutusluokka ilman interfacea.

Samalla tavalla toimivat `AddScoped` sekä `AddSingleton`, kullakin on myös useita overloadeja, esim. funktio joka palauttaa toteuttavan objektin.

[Lisätietoja: Introduction to Dependency Injection in ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection)

## Identity Core käyttäjä- ja roolienhallintakirjasto

## Rajapinta SDK:n generointi MVC:n ApiExplorer kirjastolla

