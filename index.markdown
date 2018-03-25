# C# ja ASP.NET Core 2.0

ASP.NET Core on Microsoftin tekemä web-palvelinohjelmointiin tarkoitettu ohjelmistokehys. Ensimmäinen versio tuli 2016, ja uusin versio tällä hetkellä on 2.0 joka on julkaistu 2017 elokuussa. ASP.NETistä on vanhempi versio jota on kehitetty vuodesta 2002 mutta ASP.NET Core on kokonaan uudelleenkirjoitettu versio joka ei ole taaksepäin yhteensopiva.

Samalla Microsoft julkaisi .NET Core ohljelmointiympäristön ja työkalut jotka toimivat useassa käyttöjärjestelmässä toisin kuin vanhemmat C# .NET Framework työkalut. .NET Corelle suunnattua ohjelmaa voidaan ajaa esimerkiksi pienissä Linux virtuaalikoneissa, macOS:llä tai Windowsilla.

ASP.NET Core sisältää useita kirjastoja, joista käsittelen tärkeimpiä:

* MVC - Pääkirjasto jolla voidaan toteuttaa mm. REST rajapinnat, sisältää useita kirjastoja sisällään.
* Entity Framework Core - Microsoftin tekemä tietokantakirjasto ja ORM.
* Identity Core - Kirjautumisjärjestelmän ja käyttäjien hallintaa.

Asennan myös ohjelmaan [Swagger REST-rajapintatesterin](https://swagger.io/).

Seuraavissa kohdissa esimerkkinä rakennettu ohjelma toimii kaikilla .NET Core ympäristöillä.

Tämä dokumentti koostuu viiden esimerkin pohjalle:

* Esimerkki1.Swagger / [Esimerkkiohjelma](Esimerkki1.Swagger/)
* Esimerkki2.Tietokanta / [Esimerkkiohjelma](Esimerkki2.Tietokanta/)
* Esimerkki3.Tietokanta2 / [Esimerkkiohjelma](Esimerkki3.Tietokanta2/)
* Esimerkki4.Kirjautuminen / [Esimerkkiohjelma](Esimerkki4.Kirjautuminen/)
* Esimerkki5.SDK / Esimerkkiohjelma (kesken)

Esimerkkiohjelmissa on paljon koodia jota ei esitellä tässä dokumentissa, joten kaikilta osin esimerkkiohjelmakoodi ei ole testattua ja voi olla paikoin bugista. Ohjelmakoodi on suunniteltu siten että sille on helppo kirjoittaa yksikkötestit, mutta tässä esimerkissä ei ole yksikkötestausta.

## Asennus ja perusteita rajapinnan määrittelemiseksi

Asenna ensin .NET Core omalle koneelle: [Windows](https://www.microsoft.com/net/learn/get-started/windows), [Linux](https://www.microsoft.com/net/learn/get-started/linuxredhat) tai [MacOS](https://www.microsoft.com/net/learn/get-started/macos)

Siirry tyhjään hakemistoon, huomaa että hakemiston nimestä tulee esimerkkiohjelman nimiavaruus. Aja hakemistossa seuraava komento:

```bash
dotnet new webapi
```

Koeta että ohjelma käynnistyy ja toimii oikein:

```bash
dotnet run
```

Nyt ruudulle pitäisi tulla seuraavan kaltainen tuloste

```text
Using launch settings from ...
Hosting environment: Production
Content root path: ...
Now listening on: http://localhost:5000
Application started. Press Ctrl+C to shut down.
```

**Huomio!** Aseta ympäristömuuttuja `ASPNETCORE_ENVIRONMENT=Development` jotta dotnet käynnistyy development tilassa ja aja komento uudestaan. 

Ohjelman käynnistyessä pitäisi näkyä `Hosting environment: Development` kun ohjelma ajetaan development tilassa. Ympäristömuuttuja kannattaa asettaa globaalisti, yleensä vasta tuotannossa jätetään tämä pois.

Voit nyt navigoida selaimella suoraan osoitteeseen: [http://localhost:5000/api/Values/](http://localhost:5000/api/Values/), portti saattaa vaihdella, sivun pitäisi näyttää JSON arvo `["value1","value2"]`.

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
Tässä metodissa konfiguroidaan ohjelmasi servicet, eli dependency injection riippuvuudet. Tästä tarkemmin alapuolella. Tästä on myös variantti `Configure{EnvironmentName}Services` jota kutsutaan esim development tilassa `ConfigureDevelopmentServices`.

#### `void Configure(IApplicationBuilder app, IHostingEnvironment env)` metodi
Täällä konfiguroidaan ohjelmasi HTTP pipeline, esimerkiksi middlewaret, jotka liittyvät HTTP Requestin kulkuun järjestelmän lävitse. Tästä on myös variantti `Configure{EnvironmentName}` jota kutsutaan esim development tilassa `ConfigureDevelopment`.

Asentamasi lisäkirjastot yleensä konfiguroidaan jommassa kummassa metodissa, yleinen konventio on että lisäkirjastot konfiguroidaan extensio metodeilla. `Configure`-metodissa lisäkirjastojen extensiot ovat nimetty `app.UseJokinToinenKirjasto()` ja `ConfigureServices`-metodissa `services.AddJokinToinenKirjasto()`.

Lisäkirjastojen konfiguroinnissa Use ja Add lausekkeiden järjestyksellä on tärkeä merkitys, middlewaret rekisteröityvät antamassasi järjestyksessä. Esimerkiksi autentikointiin liittyvät saattaa olla pakollista rekisteröidä ensin jotta ne toimivat oikein.


### Esimerkk1.csproj

Projektitiedosto, jossa riippuvuudet ja kääntämiseen liittyviä asetuksia on määritelty. Microsoft kehitti uuden, pienemmän projektitiedosto formaatin aikasemman tilalle, tämä on suhteellisen helposti käsin hallittavissa oleva xml tiedosto. Visual Studio osaa suurimmalti osin tiedoston hallinnan graafisesti, mutta sitä joutuu vielä käsittelemään usein suoraan tekstieditorilla.

ASP.NET Core 2:ssa kaikki riippuvuudet ovat metapaketissa `Microsoft.AspNetCore.All`. Vaikka riippuvuus on sidottu pakettiin joka viittaa kaikkiin kirjastoihin, ylimääräiset riippuvuudet [poistetaan automaattisesti](https://andrewlock.net/the-microsoft-aspnetcore-all-metapackage-is-huge-and-thats-awesome-thanks-to-the-net-core-runtime-store-2/) käännön yhteydessä.

### dotnet watch

Vielä julkaisemattomassa ASP.NET Core 2.1.4 tämä työkalu tulee valmiiksi mukana joten sitä ei tarvitse asentaa erikseen. Tämän työkalun tarkoituksena on kääntää ja käynnistää ohjelma automaattisesti uudestaan kun ohjelmakoodia muokataan, tämä nopeuttaa työtäsi kun muokkaat ohjelmaa.

Asentamista varten projektitiedostoon (csproj) pitää lisätä yksi xml elementti:

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

### Rajapinnan määrittely Routing, Controller ja action

Rajapinta määritellään ohjainluokilla (Controller), yleensä luokat ovat muotoa esimerkiksi `ValuesController` jossa `Values` saattaa viitata osaan rajapinnan HTTP osoitetta, mutta se ei ole välttämätöntä. Actionilla viitataan HTTP kyselyn käsittelevään metodiin kyseisessä luokassa.

Rajapinnan osoitteen alkuosan muodosta `Route` attribuutti, ja loppuosan actionin määrittelevä `HttpGet` tai esim `HttpPost`, sisään tuleva arvo merkataan attribuutilla `FromQuery` tai `FromBody` esimerkiksi voit korvata **ValuesController.cs** tiedoston seuraavalla:

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

Route attribuutissa on muutamia maagisia muuttujia, kuten `[controller]` ja `[action]` jotka viittaavat ohjaimen nimeen ja kyseisen actionin nimeen. Samoin sisään tulevia parametrejä voi ripotella `{muuttuja}` tai valinnaiset muuttujat `{muuttuja?}` merkinnällä.

Luokan ei tarvitse periytyä `Controller` luokasta, esimerkiksi jos haluat tehdä protokolla agnostisen API:n, se on mahdollista ilman perintää. Esimerkkinä voisi toteuttaa ohjaimet niin että niiden actionit voisi toimia HTTP:n lisäksi websocketin kautta.

Rajapinta osaa palauttaa ja ottaa sisäänsä suoraan dataa JSON muodossa, mutta kannattaa pysytellä tyyppiturvallisissa palautus ja sisäänotto muodoissa, eli helposti serialisoitavissa perusluokissa.

[Lisätietoja: Routing to Controller Actions](https://docs.microsoft.com/en-us/aspnet/core/mvc/controllers/routing)

### Rajapintatesteri Swagger

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

[Voit myös tarkastella tämän esimerkin ohjelmakoodeja: Esimerkki1.Swagger](Esimerkki1.Swagger/)

## Tietokantaesimerkki 1. - perusesimerkki Entity Framework Core tietokantakirjastolla

ASP.NET Core ohjelmistokehys ei pakota käyttämään tiettyä tietokantakirjastoa, mutta Entity Framework Core (EF Core) kirjasto toimii parhaiten ASP.NET Coren kanssa kanssa valmiiksi. EF Core on Microsoftin tekemää tietokantakirjasto joka osaa mm. luoda tietokannan luokkien pohjalta, sisältää Object Relational Mapperin (ORM) ja osaa muuntaa C# LINQ kieltä SQL lauseiksi.

Ensin luodaan yleensä hakemisto ja namespace `Models` jonne tietokannan malli rakennetaan.

Toteutan seuraavana pienen usean käyttäjän laskujen ylläpitojärjestelmän jossa yritys voi rekisteröityä sähköpostilla ohjelmaan. Yritys voi ohjelmassa lisätä, poistaa, ja muokata laskuja sekä asiakkaita. Ensimmäinen esimerkki pyrkii rakentamaan vain tietokannan ja näyttämään miten sitä voi kysellä REST rajapinnalla, tämä perusesimerkki on vajaavainen esimerkki. Seuraavassa esimerkissä jatketaan jalostamalla tätä paremmaksi.

Tietokantamallini on seuraava:

![Tietokanta models](tietokanta-models.png)

* ApplicationUser on ohjelman käyttäjät, tämä on yleensä nimetty näin ASP.NET tutoriaaleissa, se periytyy IdentityUser luokasta jossa on valmiiksi tiettyjä propertyjä kuten Email, Username, Password, jne.
* Business on yritys joka rekisteröityy ohjelmaan sähköpostilla
* Client on yrityksen asiakas
* Invoice on yrityksen tekemä lasku
* InvoiceRow on laskurivi
* Email on taulu sähköposteja varten

### Models/Models.cs - tietokannan taulujen määrittely

```cs
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Esimerkki2.Tietokanta.Models
{
    // Nämä modelit ovat tässä samassa tiedostossa jotta niitä on helpompi demota,
    // oikeasti ne kannataa siirtä omiin tiedostoihinsa kuten C#:ssa on tapana

    public class ApplicationUser : IdentityUser<int> // Primary keyn tyyppi Int
    {
        // IdentityUser on ASP.NET Identity kirjaston yläluokka käyttäjiä
        // varten.
        //
        // Tänne periytyy propertyjä kuten Username, Email, Password, ... vaikka
        // et tarvisi kaikkia propertyjä jotka tänne periytyy, niin ne kannatta
        // säilyttää jotta ei tule päänsärkyä Identity kirjaston kanssa
        // yhteensopivuudesta, kaikkia propertyjä ei ole pakko käyttää omassa
        // toteutuksessa
    }

    public class ApplicationRole : IdentityRole<int> {
        // Käyttäjän rooli ohjelmassa, en käytä tätä tässä esimerkissä
    }

    public class Business
    {
        public int Id { get; set; }
        public string Title { get; set; }

        // Suora viittaus
        public int OwnerApplicationUserId { get; set; }
        public ApplicationUser OwnerApplicationUser { get; set; }
    }

    public class Client
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string PostCode { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }

        // Suora viittaus
        public int BusinessId { get; set; }
        public Business Business { get; set; }
    }

    public class Invoice
    {
        public int Id { get; set; }
        public string Title { get; set; }

        // Sent on oikeasti huono idea laskutusohjelmalle, lähetetty lasku pitäisi
        // viedä esim. uuteen tauluun ja arvot jäädyttää, mutta tämä on esimerkki
        public DateTime? Sent { get; set; }

        // Suora viittaus (mutta nullable huomaa "?")
        public int? ClientId { get; set; }
        public Client Client { get; set; }

        // Suora viittaus
        public int BusinessId { get; set; }
        public Business Business { get; set; }

        // Väärinpäin oleva navigointi (Inverse navigation), viittaa tämän
        // laskun InvoiceRow listaan
        public IList<InvoiceRow> InvoiceRows { get; set; }

        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
    }

    public class InvoiceRow
    {
        public int Id { get; set; }

        // Suora viittaus
        public int InvoiceId { get; set; }
        
        [JsonIgnore] // Tämä estää referenssien loopin tässä esimerkissä
        public Invoice Invoice { get; set; }

        public string Name { get; set; }
        public int Quantity { get; set; }
        public Decimal Amount { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
    }

    public class Email
    {
        public int Id { get; set; }
        public string To { get; set; }
        public string From { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
    }
}
```

Tietokantamalli luodaan siis tekemällä normaaleja luokkia. EF Core luo näistä tietyin konventioin tietokantataulut. `Int` tyyppinen `Id` kenttä on `primary key` ja auto increment. Viittauskentät toisiin tauluihin on nimetty `ToinenLuokkaId` eli luokannimi johon viitataan ja Id perään, näistä tulee `foreign key`. Konventioihin voi vaikutaa `DbContext` luokalla.

### AppDbContext.cs - tietokannan käsittelyluokka

Tietokannan käsittelyä ja yhdistämistä varten tarvitaan `DbContext` luokasta periytyvä luokka jossa määrätään taulut ja mahdolliset lisäasetukset kullekkin taululle, esim erikoiset avaimet tms.

```cs
using Esimerkki2.Tietokanta.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Esimerkki2.Tietokanta.Db
{
    public class AppDbContext : DbContext
    { 
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        
        // Taulujen nimiksi tulee seuraavien propertyjen nimet, esim "Business" tai "Client"
        public DbSet<Business> Business { get; set; }
        public DbSet<Client> Client { get; set; }
        public DbSet<Invoice> Invoice { get; set; }
        public DbSet<InvoiceRow> InvoiceRow { get; set; }
        public DbSet<Email> Email { get; set; }

        protected override void OnModelCreating(ModelBuilder builder) {
            // Täällä voi antaa eri asetuksia tauluille
            // Esim. seuraava antaisi yhdistetyn avaimen
            // builder.Entity<Business>().HasAlternateKey(t => new { t.Id, t.OwnerApplicationUserId })
            
            // Tai seuraavalla voisi antaa kummallisen nimen taululle
            // builder.Entity<Business>().ToTable("OUTOLINTU");
        }
    }
}
```

### Startup.cs - tietokantaan yhdistäminen ja testidata

Seuraavaksi määritellään tietokanta johon yhdistetään, tässä käytetään SQLiteä esimerkkinä. Tarkoitus on rekisteröidä `AppDbContext` mm. riippuvuusinjektiota varten, lisää `ConfigureServices()` metodiin seuraavat rivit:

```cs
services.AddDbContext<AppDbContext>(o => {
    // SQLite tietokannan nimi
    o.UseSqlite("Data Source=esimerkki.development.db;");
});
```

Tämä määrittelee että `AppDbContext` instanssin täytyy yhdistää `esimerkki.development.db` tiedostossa olevaan tietokantaan.

Tietokanta täytyy luoda koska sitä ei ole vielä luotu, sitä varten listäään `Configure()` metodiin seuraavat rivit:

```cs
// AppDbContextin voi luoda vain scopen sisällä, joten ensin luodaan scope
using (var scoped = app.ApplicationServices.CreateScope()) {
    var dbContext = scoped.ServiceProvider.GetRequiredService<AppDbContext>();
    var passHasher = scoped.ServiceProvider.GetRequiredService<IPasswordHasher<ApplicationUser>>();
    
    // Configure on synkroninen, joten tässä pitää odotella
    CreateTestData(dbContext, passHasher).GetAwaiter().GetResult();
}
```

Luodaan nyt myös testidataa ja tietokanta, tässä esimerkissä tietokanta luodaan joka käynnistyksellä uudestaan:

```cs

private async Task CreateTestData(AppDbContext appDbContext, 
    IPasswordHasher<ApplicationUser> passwordHasher)
{
    // Tuhoaa ja poistaa tietokannan joka kerta
    await appDbContext.Database.EnsureDeletedAsync();
    await appDbContext.Database.EnsureCreatedAsync();

    // Luo testidata development tilalle ...

    var acmeUser =  new ApplicationUser() {
        Email = "business@example.com",
        UserName = "business@example.com",
        PasswordHash = passwordHasher.HashPassword(null, "!Pass1"), // Testi käyttäjän salasana
        NormalizedEmail = "business@example.com".ToUpper(),
        NormalizedUserName = "business@example.com".ToUpper(),
        ConcurrencyStamp = Guid.NewGuid().ToString(),
        SecurityStamp = Guid.NewGuid().ToString(),
        EmailConfirmed = true,
    };

    var acmeBusiness = new Business() {
        Title = "Acme Inc",
        OwnerApplicationUser = acmeUser,
    };

    var clients = new List<Client>() {
        new Client() {
            Business = acmeBusiness,
            Name = "Kukkaismyynti Oy",
            Address = "Kukkaiskuja 3",
            City = "Jyväskylä",
            PostCode = "40100",
            Email = "kukkaismyynti@example.com",
            PhoneNumber = "+3585012341234"
        },
        new Client() {
            Business = acmeBusiness,
            Name = "Kynäkauppiaat Ry",
            Address = "Kynäkatu 123",
            City = "Helsinki",
            PostCode = "00100",
            Email = "kynakauppias@example.com",
            PhoneNumber = "+3585043214321"
        }
    };

    var invoices = new List<Invoice>() {
        new Invoice() {
            Business = acmeBusiness, 
            Client = clients[0],
            Title = "Lasku ruusupuskista",
            InvoiceRows = new List<InvoiceRow>() {
                new InvoiceRow() {
                    Amount = 15.0M,
                    Created = DateTime.Now,
                    Modified = DateTime.Now,
                    Name = "Ruuspuskan siemenet",
                    Quantity = 123
                }
            },
            Modified = DateTime.Now,
            Created = DateTime.Now,
            Sent = null,
        }
    };

    appDbContext.Business.Add(acmeBusiness);
    appDbContext.Client.AddRange(clients);
    appDbContext.Invoice.AddRange(invoices);
    
    await appDbContext.SaveChangesAsync();
}
```

Yllä luodaan siis olioita ja ne lisätään AppDbContextiin jonka jälkeen se tallennetaan tietokantaan. Tietokanta tyhjennetään ja luodaan joka käynnistyksellä uudelleen, tämä luo helpon kehitysympäristön jossa data on aina samassa tilassa joka käynnistyksellä.

### InvoicesController.cs - tietokantahaun esimerkki

Luodaan nyt esimerkkinä `InvoicesController` jolla esittelen yksinkertaista tietokantahakua rajapinnalle.

```cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Esimerkki2.Tietokanta.Db;
using Esimerkki2.Tietokanta.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Esimerkki2.Tietokanta.Controllers
{
    [Route("api/[controller]")]
    public class InvoicesController
    {
        private readonly AppDbContext dbContext;

        public InvoicesController(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpGet("{id}")] 
        public async Task<Invoice> GetById(int id)
        {   
            // Tässä esimerkissä palautetaan modeli, normaalisti sitä ei
            // tehtäisi vaan jokaiselle end pointille määritellään oma
            // palautusluokka, modelit eivät saa vuotaa rajapinnalle
            return await dbContext.Invoice.Where(t => t.Id == id)
                .Include(t => t.InvoiceRows)
                .FirstOrDefaultAsync();
        }
    }
}
```

Nyt voit koittaa Swaggerillä, tai curlilla `curl -X GET 'http://localhost:5000/api/Invoices/1'`, ulos pitäisi tulla seuraavankaltainen JSON objekti:

```json
{
  "id": 1,
  "title": "Lasku ruusupuskista",
  "sent": null,
  "clientId": 1,
  "client": null,
  "businessId": 1,
  "business": null,
  "invoiceRows": [
    {
      "id": 1,
      "invoiceId": 1,
      "name": "Ruuspuskan siemenet",
      "quantity": 123,
      "amount": 15,
      "created": "2018-02-17T23:14:42.5338954",
      "modified": "2018-02-17T23:14:42.5339859"
    }
  ],
  "created": "2018-02-17T23:14:42.5342508",
  "modified": "2018-02-17T23:14:42.5341969"
}
```

### ClientsController.cs - tallennus tietokantaan, ja syötteen validointi esimerkki

Datan validointiin C#:ssa on suhteellisen vakiintunut konventio käyttää `System.ComponentModel.DataAnnaotations` attribuutteja. Validointi on myös yksi hankalimmisa web apiin liittyvistä ongelmista, joten tällekkin on useita kirjastoja kuten tunnettu [FluentValidation kirjasto](https://github.com/JeremySkinner/FluentValidation), en kuitenkaan käytä sitä tässä esimerkissä.

```cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Esimerkki2.Tietokanta.Db;
using Esimerkki2.Tietokanta.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Esimerkki2.Tietokanta.Controllers
{
    [Route("api/[controller]")]
    public class ClientsController : Controller
    {
        private readonly AppDbContext dbContext;

        public ClientsController(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public class CreateClientDto {
            [MinLength(3)]            
            public string Name { get; set; } = "";
            public string Address { get; set; } = "";
            public string City { get; set; } = "";

            [RegularExpression("(\\d{5})?")]
            public string PostCode { get; set; } = "";

            [EmailAddress]
            public string Email { get; set; } = "";

            public string PhoneNumber { get; set; } = "";
        }

        // Huomaa että palautusmuoto on object, joka on huono, mutta tämä on
        // yksinkertaisin esimerkki
        [HttpPost] 
        public async Task<object> Create([FromBody] CreateClientDto createClientDto)
        {   
            if (!ModelState.IsValid) {
                // Tälle tulee parempi vaihtoehto seuraavassa esimerkissä
                return ModelState;
            }
            // Business ID on vakiona 1 tässä esimerkissä
            var businessId = 1;

            var newClient = new Client() {
                City = createClientDto.City,
                Address = createClientDto.Address,
                Name = createClientDto.Name,
                PostCode = createClientDto.PostCode,
                Email = createClientDto.Email,
                BusinessId = businessId,
            };
            dbContext.Client.Add(newClient);
            await dbContext.SaveChangesAsync();
            return newClient;
        }
    }
}
```

`ModelState.IsValid` tarkistaa että syötteenä oleva `CreateClientDto` ei sisälä virheitä. Koeta virheentarkistuta swaggerilla, esimerkiksi seuraavalla syötteellä:

```json
{
  "name": "Matti Meikäläinen",
  "address": "Kukkaiskuja 23",
  "city": "Jyväskylä",
  "postCode": "virhepostikoodi",
  "email": "virheposti",
  "phoneNumber": ""
}
```

Tulee vastaus esimerkiksi:

```json
{
  "Email": {
    "key": "Email",
    "errors": [
      {
        "errorMessage": "The Email field is not a valid e-mail address."
      }
    ]
  },
  "PostCode": {
    "key": "PostCode",
    "errors": [
      {
        "errorMessage": "The field PostCode must match the regular expression '(\\d{5})?'."
      }
    ]
  }
}
```
Yllä olevasta JSON:sta on leikattu epäoleelliset kentät pois, siinä on useita kenttiä joita et ehkä tarvitse. Tätä varten kannattaa luoda oma luokka joka ottaa sisäänsä `ModelStateDictonary` ja muuttaa sen JSON:iksi josas on vain oleelliset kentät.

Jos syöte on oikea luodaan uusi asiakas.

[Voit myös tarkastella tietokannan perusesimerkkin ohjelmakoodeja: Esimerkki2.Tietokanta](Esimerkki2.Tietokanta/)


## Dependency Injection ohjelmakirjaston ymmärtäminen

ASP.NET Core käyttää Microsoftin tekemää Dependency Injection -kirjastoa hallitsemaan eri osien riippuvuuksia. Tarkoituksena on kirjoittaa riippuvuudet palveluina (Service), ja järjestelmä hoitaa näiden luomisen automaattisesti. Samaan tapaan kuin esim. Javassa Google Guice tai PHP:ssä Laravel Service container.

Injektiolla voidaan helpottaa mm. testausta, ja mahdollistaa toteutuksen vaihtamisen suhteellisen helposti niissä osissa missä se on tarpeen.

Riippuvuudet rekisteröidään seuraavalla kolmella tavalla **Startup.cs** tiedoston `ConfigureServices` metodissa:

* `services.AddTransient` - Riippuvuudet jotka ovat elossa mahdollisimman lyhyen ajan, eli ne luodaan kullekkin kohteelle uudelleen.
* `services.AddScoped` - Riippuvuus joka on elossa yhden HTTP kyselyn ajan. Scopeja voi luoda myös muitakin, mutta ASP.NET Coressa yleensä kyse on HTTP-kyselyn ajan elossa olevista olioista.
* `services.AddSingleton` - Riippuvuus joka on elossa koko ohjelman suorituksen ajan.

Kukin käsky ottaa sisäänsä rajapinnan tai luokan, sekä toteutusluokan. Esimerkkinä rekisteröidään rajapinnalle toteutusluokka:

```cs
services.AddTransient<IEmailSender, EmailSender>()
```

Eli rajapinta on esimerkiksi:

```cs
public interface IEmailSender {
    Task<Email> SendEmailAsync(Email email);
}
```

Sekä toteutukselle luokka, tässä esimerkissä ei lähetä sähköpostia vaan se pelkästään tallennetaan `Email` tietokantatauluun. Vaikka toteutus lähettäisi sähköpostin se kannattaa myös tallentaa sähköpostitauluun, jotta voi helpommin seurata lähetettyjä sähköposteja tai etsiä virheitä:

```cs
public class EmailSender : IEmailSender {
    private readonly AppDbContext dbContext;

    public EmailSender(AppDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<Email> SendEmailAsync(Email email) {
        // Tässä voisi lähettää sähköpostin oikeasti ...

        dbContext.Email.Add(email);
        await dbContext.SaveChangesAsync();
        return email;
    }
}
```

Tämän jälkeen ei tarvitse viitata `EmailSender` luokkan vaan voi käyttää vain `IEmailSender` rajapintaa injektion kohteena.

Rajapintaluokka ja toteutusluokka voivat olla samoja, erityisesti kun järjestelmää vasta kehtitetään on yleistä rekisteröidä vain toteutusluokka injektiolle ilman rajapintaa.

Samalla tavalla toimivat `AddScoped` sekä `AddSingleton`, kullakin on myös useita overloadeja, esim. funktio joka palauttaa toteuttavan objektin.

[Lisätietoja: Introduction to Dependency Injection in ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection)


## Middlewaret

Middlewaret ovat HTTP kyselyä ennen tai jälkeen ajettavia koodin pätkiä. Esimerkiksi ohjaimesta tai jostain sen sisältä tuleva virhe `NotFoundException` voitaisiin näyttää JSON objektina `{ error: "NOT_FOUND" }`, tämä käsitellään suorituksen jälkeen. Yleinen toinen esimerkki on syötteen validointi, joka tehdään ennenkö syöte tulee ohjaimelle.

Edellisessä esimerkissä jos laskua ei löytynyt palautettiin null, tämä ei ole kovinkaan hyvä tapa. Parempi on määritellä omalle rajapinnalle selkeä ja yksiselitteinen tapa palauttaa virheet.

### Virheenkäsittelyn esimerkkifiltteri

Oletetaan että tietokanta heittää virheen `NotFoundException` ja halutaan että tällöin HTTP vastaus on aina `404` ja palautteena oleva JSON on `{ error: "NOT_FOUND" }` se voitaisiin toteuttaa seuraavalla tavalla:

```cs
using System;
using Esimerkki3.Tietokanta2.Stores; // NotFoundException on määritelty täällä
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Esimerkki3.Tietokanta2.Mvc
{
    public class ApiErrorFilter : Attribute, IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            if (context.Exception is NotFoundException)
            {
                // Tämä ei ole tyyppiturvallista, mutta yksinkertaisin tapa 
                // palauttaa JSON result
                context.Result = new JsonResult(new {
                    error = "NOT_FOUND"
                });
                context.Exception = null; // Nollaa virhe
                context.ExceptionHandled = true; // Virhe käsitelty

            // Voit käsitellä tässä filtterissä muitakin exceptioneita...
            // Esim oma tekemiäsi virheitä, alla lista yksinkertaisista 
            // omista exceptioneista
            } else if (context.Exception is ApiError)
            {
                context.Result = (context.Exception as ApiError).GetResult();
                context.Exception = null;
                context.ExceptionHandled = true;
            }

        }
    }

    public class NotFound: ApiError
    {
        public NotFound() {
            StatusCode = StatusCodes.Status404NotFound;
        }
    }
    
    public class NotAuthorized: ApiError
    {
        public NotAuthorized() {
            StatusCode = StatusCodes.Status401Unauthorized;
        }
    }

    public class Forbidden : ApiError
    {
        public Forbidden()
        {
            StatusCode = StatusCodes.Status403Forbidden;
        }
    }

    // Omien virheiden yläluokka
    abstract public class ApiError : Exception
    {
        public int? StatusCode { get; set; }

        virtual public IActionResult GetResult()
        {
            return new ObjectResult(new ErrorValue<object>()
            {
                // Ottaa tyypin nimen virheobjektiin, esim "Forbidden" tai "NotFound"
                Error = GetType().Name, 
                Data = null
            })
            {
                StatusCode = StatusCode
            };
        }
    }
}
```

Yllä olevassa esimerkissä on myös määritelty muutama oma virhe, esimerkkinä.

Filtterin voi rekisteröidä millä tahansa tasolla, globaalisti, tai tietylle ohjaimelle tai käsittelijälle. Tässä ohjelmassa on kätevintä käsitellä kaikkiaalta nousevat NotFoundExceptionit joten se rekisteröidään globaalisti näin, avaa **Startup.cs**:

```cs
public void ConfigureServices(IServiceCollection services)
{    
    // ...
    services.AddMvc(o => {
        // Tänne täytyy tehdä lambda jossa filtteri konfiguroidaan
        o.Filters.Add(new ApiErrorFilter());
    });
    // ...
}
```

### Validointi middleware

On hieman työlästä kirjoittaa `if (!ModelState.IsValid) { ... }` jokaiseen käsittelijään, joten sille kannattaa myös tehdä globaalisti rekisteröity middleware. Tällöin validointi ja siitä nouseva virheviesti toteutetaan yhtenevästi koko rajapinnalle. ASP.NET Core 2.1 (ei vielä julkaistu) tämä [on toteutettu jo valmiiksi](https://blogs.msdn.microsoft.com/webdev/2018/02/27/asp-net-core-2-1-web-apis/).

```cs
using System;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Esimerkki3.Tietokanta2.Mvc
{
    public class ModelStateValidationFilter : Attribute, IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                // Tämä on turhan laaja objekti palautettavaksi, 
                // mutta se millaisen objektin palauttaa on rajapinnasta kiinni
                context.Result = new JsonResult(context.ModelState); 
            }
        }

        public void OnActionExecuted(ActionExecutedContext context) { }
    }
}
```

Tämä filtteri rekisteröidään koko ohjelman laajuisesti `AddMvc` käskyä konfiguroimalla **Startup.cs** tiedostossa näin:


```cs
public void ConfigureServices(IServiceCollection services)
{    
    // ...muut asetuksesi

    // AddMvc metodille pitää antaa konfigurointi lambda
    services.AddMvc(o => {
        o.Filters.Add(new ApiErrorFilter()); // Tämä lisätään tänne 
        o.Filters.Add(new ModelStateValidationFilter()); // Tämä lisätään tänne 
    });

    // ... muut asetuksesi
}
```


Rekisteröinnit voi suorittaa myös ohjain tai käisttelijä kohtaisesti, jos ei jostain syystä halua jotain filtteriä koko ohjelmalle. Esimerkiksi, ne voi rekisteröidä ohjain tai käsittelijäkohtaisesti:

```cs
[ModelStateValidation] // <-- tämä rekisteröi filterin vain tälle ohjaimelle
public class ClientsController : Controller {

    [ModelStateValidation] // <-- tämä rekisteröisi filterin vain tälle actionille
    public async Task<object> Create([FromBody] CreateClientDto createClientDto) {
        // ...
    }
}
```

## Tietokantaesimerkki 2. - Repository pattern, palvelut, DTO

Pienemmissä ohjelmissa `AppDbContext` tietokantakäsittelijää voi kutsua ohjaimesta suoraan, mutta SQL kyselyiden, ja liiketoimintalogiikan (Business Logic) sekoittaminen ohjaimiin (Controller) tekee asioista hyvin hankalaa isommissa ohjelmistoissa.

Tarkoitus on jakaa ohjelma selkeisiin osiin haasteiden eriyttämisellä (separation of concerns):

* Ohjaimet validoivat käyttäjältä tulevan syötteen ja kutsuvat palveluja. 
* Palvelut käyttävät toisia palveluita ja kutsuvat storeja. 
* Storet tallentavat ja hakevat malleja tietokannasta.

`Controller <-> Service <-> Store`

Tämä esimerkki esittelee miten ohjelma alkaisi rakentumaan. Tähän on otettu vain esimerkki kustakin tasosta, loput osat voi tarkastella ohjelmakoodeista. Muut osat ovat samojen esimerkkien toistamista suurilta osin. Esimerkin ohjelmakoodeissa on paljon muitakin pieniä parannuksia, joita en esittele tässä dokumentissä, esim. testidata generoidaan tietokantaan, tai tietokantayhteysasetukset haetaan asetustiedostosta. Näitä kannattaa katsoa esimerkkikoodeista.

### Repository pattern (Stores)

Tietokannan käsittely kannattaa siirtää omiin luokkiinsa, käytän tässä repository patternin kaltaista suunnittelumallia, store jälkiliitettä käytetään esimerkiksi Identity kirjaston luokissa. Luokat ovat seuraavat esimerkkiohjelmassani:

* `InvoiceStore` - laskut ja laskurivit
* `BusinessStore` - yritykset
* `ClientStore` - asiakkaat
* `EmailStore` - sähköpostit

Kussakin luokassa on kaikki tietokantaan tehtävät kyselyt, monesti tänne voi tulla kummallisia ja pitkästi nimettyjä metodeja, mutta se ei haittaa. Tarkoituksenani on tehdä metodit siten että SQL-kysely tapahtuu aina Storejen sisällä eli palautusarvot ovat konkreettisia, toinen vaihtoehtoinen tapa olisi palauttaa `IQueryable` jota voi vielä muokata.

Tämä suunnittelumalli ei ole sopiva jos halutaan tietotaulujen (DataGrid) kaltaista rakennetta, tai muuten hyvin dynaamisia kyselyitä esim. hakuja varten. GraphQL on mahdollisesti parempi rajapinta jos tietoa tarvii kysellä useassa muodossa, mutta sille olevat kirjastot eivät ole vielä tarpeeksi kehittyneitä C#/.NET ympäristössä.

#### `InvoiceStore.cs` esimerkkinä

Tämä store tallentaa, lukee ja hakee ohjelman laskuja, voit katsoa muut Storet esimerkkikoodeista. En käytä tässä mitään yleistystä, perintää, tai koodin generointia CRUD toimintojen tekemiseksi. 

Tässä on pienenä poikkeuksena se että tämä store hallitsee myös kyseisen laskun laskurivien tallentamisen.

```cs
public class InvoiceStore
{
    private readonly AppDbContext dbContext;

    public InvoiceStore(AppDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task Update(Invoice invoice) {
        invoice.Modified = DateTime.UtcNow;
        var newRowIds = invoice.InvoiceRows.Select(t => t.Id).ToArray();

        // Update:n kutsuminen ei itseasiassa ole pakollista, sillä EF Core
        // osaa hallita olioiden muutoksia sisäisesti. Sisäinen hallinta voi
        // olla joskus varsin ongelmallista, sillä ei ole helposti
        // ennustettavissa mitä seuraava SaveChangesAsync() tekee.
        dbContext.Invoice.Update(invoice); 

        var removedRows = await dbContext
            .InvoiceRow
            .Where(t => 
                t.InvoiceId == invoice.Id &&
                !newRowIds.Contains(t.Id)
            )
            .ToListAsync();

        dbContext.RemoveRange(removedRows);
        
        // Tämä ajaa SQL kyselyt ja siihen asti kerätyt muutokset
        await dbContext.SaveChangesAsync();
    }

    public async Task<Invoice> Create(Invoice invoice) {
        invoice.Created = DateTime.UtcNow;
        invoice.Modified = DateTime.UtcNow;
        dbContext.Invoice.Add(invoice);
        await dbContext.SaveChangesAsync();
        return invoice;
    }

    public async Task Remove(Invoice invoice) {
        dbContext.Invoice.Remove(invoice);
        await dbContext.SaveChangesAsync();
    }

    public async Task<Invoice> GetByBusiness(int businessId, int id) {
        var value = await dbContext.Invoice
            .Include(t => t.InvoiceRows)
            .Include(t => t.Client)
            .FirstOrDefaultAsync(t => t.Id == id && t.BusinessId == businessId);

        value.InvoiceRows = value.InvoiceRows.OrderBy(t => t.Sort).ToList();

        if (value == null) {
            throw new NotFoundException();
        }
        return value;
    }

    public async Task<ICollection<Invoice>> ListLatestByBusiness(int businessId) {
        return await dbContext.Invoice
            .Include(t => t.InvoiceRows) // Sisällytä kyselyyn laskurivien tiedot
            .Include(t => t.Client) // Sisällytä kyselyyn asiakkaantiedot
            .Where(t => t.BusinessId == businessId)
            .OrderBy(t => t.Created)
            .ToListAsync();
    }
}
```

Lstauksen näyttömisessä käytetään EF Coren `Include` metodia, joka palauttaessaan rivin täyttää myös rivin viittaukset arvoilla. EF Coressa ei ole vielä viitteiden laiskaa latausta, vaan ne pitää itse muistaa listata jos niitä tarvitsee.

### Palvelut

Tarkoitus on luoda palvelut (Service) ja toiminnot kullekkin ilmeiselle käyttötapaukselle:

* Yritykset rekisteröityvät
    * `BusinessService.Register()`
* Yritys lisää, poistaa ja muokkaa omia asiakkaitaan
    * `ClientService.Create()`
    * `ClientService.Remove()`
    * `ClientService.Update()`
    * `ClientService.ListByBusiness()`
* Yrityksen pitää pystyä listaamaan, näyttämään, lisäämään, tuhoamaan, muokkaamaan, ja lähettämään laskujaan
    * `InvoiceService.Create()`
    * `InvoiceService.Remove()`
    * `InvoiceService.Update()`
    * `InvoiceService.Send()`
    * `InvoiceService.GetByBusiness()`
    * `InvoiceService.ListLatestByBusiness()`
* Käyttäjän pitää pystyä muuttamaan ja resetoimaan salasana
    * `AccountService.ChangePassword()` vaihtaa salasanan omasta toimesta
    * `AccountService.ForgotPassword()` lähettää resetointi osoitteen sähköpostiin
    * `AccountService.ResetPassword()` ottaa salasana resetoinnin tokenin ja vaihtaa salasanan
* Järjestelmän pitää pystyä lähettämään ilmoituksia
    * `NotificationSender.SendBusinessRegistered()` - yrityksen rekisteröitymissähköposti
    * `NotificationSender.SendForgotPassword()` - unohditko salasanasi? resetointilinkki
    * `NotificationSender.SendInvoice()` - lähetä lasku

Huomaa että esimerkiksi `InvoiceService` ja sen rajapinta saattaa näyttää hyvin samalta mitä `InvoiceStore` mutta ne ovat eri tasoilla, ja toteuttavat eri asiaa ohjelmassa. Vaikka tässä esimerkissä `InvoiceService` on hyvin tyhmä ja kutsuu melkein suoraan storea, niin yleensä ohjelmiston kasvaessa palvelutasolla olevat luokat tekevät paljon enemmän. 

ASP.NET Coressa käyttäjien oikeustarkistus alkaa ohjaintasolla joten palvelutasolla ei ole erikseen oikeustarkistusta toiminnoissa. Oikeustarkistus ja kirjautumisjärjestelmän integrointi käsitellään vasta seuraavassa esimerkissä.

#### `InvoiceService` esimerkkinä

```cs
public class InvoiceService
{
    private readonly InvoiceStore invoiceStore;
    private readonly NotificationSender notificationSender;

    public InvoiceService(InvoiceStore invoiceStore, NotificationSender notificationSender)
    {
        this.invoiceStore = invoiceStore;
        this.notificationSender = notificationSender;
    }

    public async Task Update(Invoice invoice) {
        await invoiceStore.Update(invoice);
    }

    public async Task<Invoice> Create(Invoice invoice) {
        return await invoiceStore.Create(invoice);
    }

    public async Task Remove(Invoice invoice) {
        await invoiceStore.Remove(invoice);
    }

    public async Task<Invoice> Send(Invoice invoice) {
        invoice.Sent = DateTime.UtcNow;
        await invoiceStore.Update(invoice);
        await notificationSender.SendInvoice(invoice);
        return invoice;
    }

    public async Task<Invoice> GetByBusiness(int businessId, int id) {
        return await invoiceStore.GetByBusiness(businessId, id);
    }

    public async Task<ICollection<Invoice>> ListLatestByBusiness(int businessId) {
        return await invoiceStore.ListLatestByBusiness(businessId);
    }
}
```

### Service ja Storen rekisteröinti riippuvuusinjektiolle

Servicet ja Storet täytyy rekisteröidä Dependency Injection kirjastolle, tämä tapahtuu muokkaamalla **Startup.cs** tiedostoa ja lisäämällä ne `ConfigureServices()` metodiin:

```cs
// Services
services.AddTransient<AccountService, AccountService>();
services.AddTransient<BusinessService, BusinessService>();
services.AddTransient<ClientService, ClientService>();
services.AddTransient<IEmailSender, EmailSender>();
services.AddTransient<InvoiceService, InvoiceService>();
services.AddTransient<NotificationSender, NotificationSender>();

// Stores
services.AddTransient<BusinessStore, BusinessStore>();
services.AddTransient<ClientStore, ClientStore>();
services.AddTransient<EmailStore, EmailStore>();
services.AddTransient<InvoiceStore, InvoiceStore>();
```

Tässä esimerkissä kaikki Servicet ovat transientteja, eli ne luodaan jokaiselle vaatimukselle uudestaan.


### DTO luokat

Koska malliolioita ei saa päästää rajapinnalle, niitä varten luodaan Dto-luokat (Data Transfer Object), näitä vastaisi näkymämallit (ViewModel) näkymäpohjaisessa sovelluksessa. Dto:n luominen voi vaikuttaa ensialkuun kovin työläiltä, koska niissä on hyvin paljon samoja kenttiä kuin malliluokissa.

Dto luokat täytyy myös pystyä muuttamaan takaisin malleiksi, ja malleista takaisin Dto luokiksi. Tätä varten voi käyttää [AutoMapper](http://automapper.org/) kirjastoa, mutta tässä esimerkissä en käytä mitään kirjastoa vaan kirjotan vastineet käsin.

Kielissä joissa tyyppiturvallisuus voidaan taata esimerkiksi tyyppi-inferenssillä, DTO luokkaa ei tarvitsisi toteuttaa vaan voisi tehdä pelkän transformaatiofunktion. Funktio palauttaisi vahvastityypitetyn anonyymin recordin, tai validointivirheen.

#### InvoiceDto.cs - Laskun palauttava DTO

Rajapinnasta ulos tuleva luokka joka määrittelee näkyvät kentät. Malliolio `Invoice` täytyy pystyä muuntamaan palautettavaksi luokaksi joten tähän on tehty staattinen rakentaja `FromInvoice()` joka luo `InvoiceDto`:n mallioliosta.

```cs
public class InvoiceDto {
    public int Id { get; set; }

    public string Title { get; set; } = "";
    public DateTime? Sent { get; set; }

    public int? ClientId { get; set; }
    public ClientDto Client { get; set; }
    public List<InvoiceRowDto> InvoiceRows { get; set; } = new List<InvoiceRowDto>();
    public DateTime Created { get; set; }
    public DateTime Modified { get; set; }

    public static InvoiceDto FromInvoice(Invoice invoice) {
        // Tässä kannattaisi käyttää AutoMapper kirjastoa eikä kirjoittaa näitä käsin
        return new InvoiceDto() {
            Id = invoice.Id,
            Title = invoice.Title,
            Sent = invoice.Sent,
            ClientId = invoice.ClientId,
            Client = ClientDto.FromClient(invoice.Client),
            Created = invoice.Created,
            Modified = invoice.Modified,
            InvoiceRows = invoice.InvoiceRows
                .Select(t => InvoiceRowDto.FromInvoiceRow(t))
                .ToList()
        };
    }

}
```

#### InvoiceUpdateDto.cs - Laskun päivitys DTO

Tarkoituksena on luoda luokka joka otetaan vastaan kun muokataan laskua, tässä on vain ne kentät jotka muokkauksessa on sallittua muuttaa. Esimerkiksi täällä ei ole `Created` tai `Sent` kenttiä. Tämän luokan kentät täytyy myös pystyä päivittämään itse oikeaan malliolioon `Invoice`, joten tätä varten on metodi `YpdateInvoice(Invoice invoice)` joka siirtää muutokset.

```cs
public class InvoiceUpdateDto {

    [MinLength(3)]
    public string Title { get; set; } = "";

    public int? ClientId { get; set; }
    public List<InvoiceRowDto> InvoiceRows { get; set; } = new List<InvoiceRowDto>();

    public Invoice UpdateInvoice(Invoice invoice) {
        invoice.Title = Title;
        invoice.ClientId = ClientId;
        
        // Korvaa laskurivit uusilla
        var oldRows = invoice.InvoiceRows;
        invoice.InvoiceRows = InvoiceRows.Select((updatedRowDto, i) => {
            var invoiceRow = 
                oldRows.Where(f => f.Id == updatedRowDto.Id).FirstOrDefault()
                ?? new InvoiceRow() {
                    Invoice = invoice
                };
            invoiceRow.Sort = i; // Järjestysnumero
            return updatedRowDto.UpdateInvoiceRow(invoiceRow);
        }).ToList();

        return invoice;
    }
}
```

#### InvoicesController.cs - DTO:n käyttö

Tarkoitus on siis määritellä Dto-luokkien avulla palautus ja sisäänottorajapinta, esimerkkinä tässä on laskuohjaimen `Get()` ja `Update()` actionit.

Tässä näkyy myös esimerkkinä kuinka `InvoiceService` tulee automaattisesti luontifunktiossa sisään koska se on rekisteröity riippuvuudeksi.

```cs
[Route("[controller]")]
public class InvoicesController
{
    private readonly InvoiceService invoiceService;
    private readonly ClientService clientService;

    public InvoicesController(InvoiceService invoiceService, ClientService clientService)
    {
        this.invoiceService = invoiceService;
        this.clientService = clientService;
    }

    [HttpGet("{id}")] 
    public async Task<InvoiceDto> Get(int id) {
        // Tässä esimerkissä businessId on vakio, seuraavassa esimerkissä se
        // haetaan käyttäjän tiedoista
        var businessId = 1;
        var invoice = await invoiceService.GetByBusiness(businessId, id);
        return InvoiceDto.FromInvoice(invoice);
    }

    [HttpPut("{id}")]
    public async Task<InvoiceDto> Update(
        int id, 
        [FromBody] InvoiceUpdateDto updateInvoiceDto)
    {
        // Tässä esimerkissä businessId on vakio, seuraavassa esimerkissä se
        // haetaan käyttäjän tiedoista
        var businessId = 1;
        var invoice = await invoiceService.GetByBusiness(businessId, id);

        // Tästä puuttuu oikeustarkistus että client Id on tämän yrityksen
        // asiakkaan ID. Sekä tarkistus että laskurivien ID:t eivät vaihdu
        // laskulta toiselle. Oikeustarkistukset tehdään seuraavassa
        // esimerkissä

        // Päivitä laskua dtosta
        updateInvoiceDto.UpdateInvoice(invoice);
        await invoiceService.Update(invoice);

        // Palauta päivitetty lasku, kierretään tietokannan kautta jotta
        // data päivittyy oikein
        return InvoiceDto.FromInvoice(
            await invoiceService.GetByBusiness(businessId, id)
        );
    }
}
```

[Voit myös tarkastella tämän esimerkin ohjelmakoodeja: Esimerkki3.Tietokanta2](Esimerkki3.Tietokanta2/)

## Kirjautumisesimerkki - Identity Core käyttäjä- ja roolienhallintakirjasto

Microsoftin tekemä Identity Core kirjasto on kokoelma käyttäjä- ja roolienhallintaan tarpeellisia palasia. Tämä kirjasto ei kuitenkaan sisällä toteutusta rajapintatasolla, vaan tarjoaa puitteet toteuttaa oma rajapinta. Rajapintamäärittely sekä tarkempi toiminnallisuus, kuten käyttäjän ja salasanan kysyminen, salasanan resetointi sähköpostien lähettäminen ym. jää jokaisen ohjelman rakennettavaksi.

Kirjautumisrajapinnalla, eli sillä osalle joka kysyy salasanan ja käyttäjänimen, käytän OpenId Connect standardin [Resource Owner Password Flow menetelmää](http://docs.identityserver.io/en/release/quickstarts/2_resource_owner_passwords.html?highlight=resource%20owner). Tämä on OAuth2.0 helpoin, mutta vajavainen tapa toteuttaa kirjautumisrajapinta. Käytännössä tässä esimerkissä käytetään vain OAuth2.0 osaa OpenId Connect standardista. OpenId Connect sisältää myös muita menetelmiä kuten Implicit flow joka on suositeltu tapa, mutta samalla harvinaisen monimutkainen eikä sovellu esimerkiksi.

Open Id:lle on valmis kirjasto [IdentityServer4](https://identityserver.io/), joka ei ole Microsoftin tekemä, mutta Microsoftin ja .NET Foundationin tukema kirjasto, jolla voidaan toteuttaa OpenId Connect hyvin yksinkertaisesti ASP.NET Coressa. IdentityServer4 ei ole iso riippuvuus, vaikka sitä voi käyttää myös omana palvelimena. Monesti isommissa ohjelmistoissa on oma palvelin joka luo tokenit keskitetysti, ja näitä pelkästään tarkistetaan API:n rajapinnalla. 

Tässä esimerkissä tokenien luonti ja tarkistaminen on molemmat samassa ohjelmassa jotta esimerkkiä on helppo käyttää.


### Identity Coren ja IdentityServer4 asentaminen

Asenna ensin IdentityServer4 riippuvuus:

```bash
dotnet add . package IdentityServer4.AspNetIdentity
dotnet add . package IdentityServer4.AccessTokenValidation
```

Riippuvuuksista kannattaa huomata, että `AccessTokenValidation` eli tokenien validointi on API:n riippuvuus, mutta `AspNetIdentity` on tokenien luonnin riippuvuus. Nämä voisi asentaa eri ohjelmiin ja ne silti toimisivat keskenään.

Identity Coren `ApplicationUser` ja `ApplicationRole` luokkia ei rekisteröity tietokannalle aikasemmassa esimerkissä. Tätä varten muuta aikasemman esimerkin `AppDbContext` periytymään `IdentityDbContext` luokasta:

```cs
public class AppDbContext : IdentityDbContext<
    ApplicationUser, ApplicationRole, int
> 
{
    // ... aikasemmat esimerkit
}
```

Seuraavaksi konfiguroidaan Identity Core sekä IdentityServer4, muokkaa **Startup.cs** tiedostoa ja sen `ConfigureServices()` metodia:

```cs
public void ConfigureServices(IServiceCollection services)
{
    // Autentikaation asetuksia, liittyvät JWT tokenin tarkistamiseen
    services
        .AddAuthentication(o =>
        {
            // Nämä vaaditaan koska .NET Core 2 ohjaa oletuksena
            // kirjautumisdialogiin, jota REST rajapinnoissa ei ole
            o.DefaultScheme =
                o.DefaultAuthenticateScheme =
                o.DefaultForbidScheme =
                o.DefaultChallengeScheme =
                o.DefaultSignInScheme =
                o.DefaultSignOutScheme =
                IdentityServerAuthenticationDefaults.AuthenticationScheme;
        })
        .AddIdentityServerAuthentication(options => // IdentityServer4
        {
            options.Authority = "http://localhost:5000";
            options.RequireHttpsMetadata = false;
            options.ApiName = "minunapi";
        });
        
    // Identity coren rekisteröinti
    services.AddIdentity<ApplicationUser, ApplicationRole>()
        .AddEntityFrameworkStores<AppDbContext>()
        .AddDefaultTokenProviders();

    // Nämä ovat Identity serverin konfiguraatiota, eli sen palan joka luo JWT tokeneita
    services.AddIdentityServer()
        .AddDeveloperSigningCredential() // Tämä luo `tempkey.rsa` tiedoston
        .AddInMemoryPersistedGrants()
        .AddInMemoryApiResources(new List<IdentityServer4.Models.ApiResource>() {
            new IdentityServer4.Models.ApiResource("minunapi", "Tämä ohjelma")
        })
        .AddInMemoryClients(new List<IdentityServer4.Models.Client>() {
            // Open ID Perustuu asiakasohjelmiin, eli jokainen
            // käyttöliittymä rekisteröidään API:lle. Esimerkiksi
            // mobiilisovelluksella olisi oma client ja websovelluksella
            // omansa.
            new IdentityServer4.Models.Client() {
                ClientId = "esimerkkiohjelma",
                AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                AllowOfflineAccess = true,
                ClientSecrets = {
                    // JavaScript sovelluksen client secret ei ole kovin
                    // tärkeä sillä se näkyy JavaScript ohjelman
                    // lähdekoodeissa, se vuotaa joka tapauksessa.
                    new Secret("secret".Sha256()) 
                },
                AllowedScopes = { "minunapi" }
            }
        }).AddAspNetIdentity<ApplicationUser>();

    // ... muut asetuksesi jatkuvat tässä kohti
}    
```

Lisää tämän jälkeen autentikointi sekä identityserver4 middlewaret ohjelmaasi, muokkaa samoin **Startup.cs** tiedostoa mutta `Configure()` metodia:

```cs
public void Configure(IApplicationBuilder app, IHostingEnvironment env)
{
    app.UseAuthentication(); // Identity coren asetus
    app.UseIdentityServer(); // IdentityServer4 asetus
    // Nämä molemmat tulevat ennen UseMvc() kohtaa!

    // muut asetuksesi jatkuvat tässä kohti, kuten UseMvc() ...
}
```

Asetuksissa toistuu joitakin vakioita, kuten "minunapi" joka on sinun API:lle määräämä scope. Scopeilla voidaan luoda useita oikeutettuja rajapintoja, esimerkiksi hallintakäyttöliittymällä voisi olla oma scope ja raporttirajapinnalla omansa. Asetuksissa pyörii myös yhdistysosoite, tämä kannattaa siirtää asetustiedostoon jotta se on helposti vaihettavissa.

### Testaa kirjautumista, JWT tokenin luomista ja tarkistamista

Tarkista, että ohjelma käynnistyy `dotnet run`, sekä navigoimalla [http://localhost:5000/.well-known/openid-configuration](http://localhost:5000/.well-known/openid-configuration) tämän osoitteen pitäisi aukaista JSON tuloste jossa on OpenId:seen liittyviä asetuksia. Käyttöliittymät tunnistavat palvelimesi käyttävän OpenId standardia tämän JSON dokumentin perustella. Tässä dokumentissa olevat authorization sekä token osoitteet pitää antaa myös Swaggerille jotta sekin osaa kirjautua järjestelmään.

Koeta että kirjautuminen palauttaa JWT tokenin käyttäen curlia, huomaa käyttäjänimi ja salasana seuraavassa kutsussa:

```bash
curl 'http://localhost:5000/connect/token' --data 'username=business%40example.com&password=!Pass1&scope=minunapi&grant_type=password&client_id=esimerkkiohjelma&client_secret=secret'
```

Vastaus pitäisi olla JWT/JSON objekti:

```json
{ 
  "access_token":"eyJhbGciOiJSUzI1NiIsIm...",
  "expires_in":3600,
  "token_type":"Bearer"
}
```

Vaikka tämä todistaa että kirjautuminen toimii, se ei takaa sitä että järjestelmä hyväksyy antamansa tokenin. Tämä täytyy testata erikseen, luodaan ensin `AccountController`, joka toistaiseksi palauttaa vain kirjautuneen tiedot. Kirjautumisen tarkistamiseen riittää `[Authorize]` attribuutti ohjaimessa. Sen voisi asettaa myös pelkästään tietylle käsittelijälle.

Luokassa oleva `UserManager` on Identityn luokka jonka avulla voi toteuttu mm. käyttäjän salasanan tai sähköpostien resetointitokenien luonnin ja tarkistamisen, ja paljon muita pieniä toimenpiteitä. Tässä esimerkissä sillä haetaan `ApplicationUser` olio käyttäjän vaateista eli JWT tokenista (`ClaimsPrincipal` tyyppinen olio on tässä esimerkissä JWT tokeni). Esimerkkiohjelmakoodeissa on muutamia muita käyttöjä UserManagerille, kuten salasanan vaihto ja resetointi. Näiden kaikkien esittäminen tässä dokumentissa ei ole oleellista.

```cs
[Authorize] // <-- Huomaa tämä!
[Route("[controller]")]
public class AccountController : ControllerBase
{
    private readonly UserManager<ApplicationUser> userManager;

    public AccountController(UserManager<ApplicationUser> userManager)
    {
        this.userManager = userManager;
    }

    public class LoggedInDto {
        public int Id { get; set; }
        public string Email { get; set; }
    }

    [HttpGet("[action]")]
    public async Task<object> Claims() {
        // Näyttää kirjautuneen käyttäjän vaateet, tämä demonstroi sitä mitä
        // vaateet tarkoittavat. User on ClaimsPrincipal tyyppinen olio, ei
        // siis ApplicationUser.
        if (User == null) {
            throw new Forbidden();
        }

        return User.Claims.Select(t => new { t.Type, t.Value }).ToList();
    }

    [HttpGet("[action]")]
    public async Task<LoggedInDto> LoggedIn() {
        // Näyttää kirjautuneen käyttäjän tiedot

        var loggedInUser = await userManager.GetUserAsync(User);
        if (loggedInUser == null) {
            throw new Forbidden();
        }

        return new LoggedInDto() {
            Id = loggedInUser.Id,
            Email = loggedInUser.Email
        };
    }
}
```

Tämän jälkeen voit koettaa access tokenia, ota saamasi token edellisestä curl kyselystä ja syötä se seuraavaan kyselyyn:

```bash
curl -H "Authorization: Bearer eyJhbGciOiJSUzI1N..." 'http://localhost:5000/Account/LoggedIn'
```

Palautusarvon pitäisi olla:

```json
{
    "id": 1,
    "email": "business@example.com"
}
```

Joka kertoo että kirjautuminen ja tokenin tarkistus toimivat. Koita myös hakea vaateet osoitteesta `http://localhost:5000/Account/Claims` samalla tavalla kuin LoggedIn haettiin. Tämän jälkeen koita syöttää access token myös [JWT.io palveluun](https://jwt.io/). Vaateet ovat siis IdentityServer4:n luomia kenttiä jotka lähetetään jokaisessa HTTP kyselyssä. Vaateita kannattaa luoda ohjelmakohtaisesti sillä näillä voi nopeuttaa käyttäjän tunnistamista ilman että tarvitsee hakea käyttäjää tietokannasta joka kerta. Esimerkiksi käyttäjän yrityksen ID olisi hyvä kenttä omaksi vaateeksi.

### Swaggeriin lisättävä kirjautuminen

Jotta Swaggerista on jotain hyötyä rajapintatesterinä kirjautumisen vaativissa sovelluksissa, sille täytyy antaa asetus kirjautumista varten, muokkaa **Startup.cs** tiedostoa:

```cs
public void ConfigureServices(IServiceCollection services)
{
    // Muut rojusi...

    // Tämä pitäisi olla jo täällä, sitä muokataan seuraavaksi:
    services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new Info { Title = "My API", Version = "v1" });
        c.AddSecurityDefinition("oauth2", new OAuth2Scheme() {
            Flow = "password",
            Scopes = new Dictionary<string, string> {
                { "minunapi", "Minun API oikeudet" }
            },
            TokenUrl = "http://localhost:5000/connect/token",
            AuthorizationUrl = "http://localhost:5000/connect/authorize"
        });
    });
}
```

Tämä toimii koska käytössä on OAuth2 standardin mukainen kirjautumistapa, jonka Swagger tunnistaa jos sille annetaan tieto siitä. Nyt voit koettaa kirjautumista myös Swaggerin kautta, käyttöliittymän ylänurkkaan on tullut Authorize näppäin josta voi syöttää ohjelman tarvittavat tiedot:

![Swagger v1 api](swagger-authorize.png)

Tämän jälkeen voit kutsua `Account/LoggedIn` endpointtia swaggeristä ja sen pitäisi palauttaa samat tiedot mitä `curl` yllä.

### Oikeuksien määrittely

ASP.NET Core 2:ssa on rooli, vaatimus (Claim), policy ja resurssipohjaiset [autorisointimenetelmät](https://docs.microsoft.com/en-us/aspnet/core/security/authorization/). `InvoiceController` esimerkkiä jateketaan siten, että siihen lisätään tarkistus jossa varmistetaan että käyttäjä on kirjautunut. Kirjautumisehto voidaan lisätä yksinkertaisesti `[Authorize]` attribuutilla.

Tämä ei kuitenkaan vielä hae sitä sitä mikä yritys on kirjautunut järjestelmään ja onko sillä oikeuksia tiettyyn resurssiin. Samalla attribuutilla voi määritellä policy perusteisia sääntöjä kuten käyttäjällä on muokkausoikeudet `[Authorize("EditPolicy")]`, tai vaade perusteisia esim. käyttäjä on vähintään 21 vuotta vanha `[Authorize("IsAtLeast21")]`. Authorize attribuutilla ei voi kuitenkaan antaa resurssipohjaisia ehtoja kuten saako käyttäjä muokata tiettyä laskua.

Resurssipohjaiset tarkistukset on hieman hankalalla tavalla toteutettu, jokainen vaatii oman luokan ja tarkistaminen tapahtuu oliolla joka ei ole tyyppiturvallinen. Esitän tässä suoraviivaisemman tavan toteuttaa resurssipohjainen tarkistus. Käytännössähän resurssipohjaisessa tarkistamisessa riittää saada käyttäjän vaatimukset (`ClaimsPrincipal`) ja tarkistaa onko käyttäjällä esimerkiksi oikeus laskun muokkaamiseen tai tuhoamiseen laskun ID:n perusteella.

#### InvoicesAuthorize.cs

Laskuja varten tässä esitellän esimerkkinä kaikki tarkistukset jokaiselle toiminnolle. Esimerkkikoodeissa on myös Authorize luokat `BusinessAuthorize` ja `ClientsAuthorize`, ne ovat vastaavasti yksinkertaisia luokkia jotka tarkistavat käyttäjän tiedoista oikeudet.

```cs
public class InvoicesAuthorize
{
    private readonly BusinessAuthorize businessAuthorize;
    private readonly InvoiceStore invoiceStore;
    private readonly ClientsAuthorize clientsAuthorize;

    public InvoicesAuthorize(BusinessAuthorize businessAuthorize, 
        InvoiceStore invoiceStore, ClientsAuthorize clientsAuthorize)
    {
        this.businessAuthorize = businessAuthorize;
        this.invoiceStore = invoiceStore;
        this.clientsAuthorize = clientsAuthorize;
    }

    public async Task<bool> CanUpdateInvoice(ClaimsPrincipal claims, int id, 
        InvoiceUpdateDto invoiceUpdateDto)
    {
        var businessId = await businessAuthorize.GetBusinessId(claims);

        // Tarkista että laskun omistaa oikea käyttäjä
        if (!await invoiceStore.OwnedByBusiness(businessId, id)) {
            return false;
        }

        // Tarkista että olemassaolevien laskurivien ID:t kuuluvat laskulle
        if (!await invoiceStore.InvoiceRowsBelongToInvoice(id,
            invoiceUpdateDto.InvoiceRows
                .Where(t => t.Id != null)
                .Select(t => t.Id)
                .Cast<int>()))
        {
            return false;
        }

        // Tarkista että asiakkaan ID on yrityksen asiakkaan ID
        if (invoiceUpdateDto.ClientId != null && 
            !await clientsAuthorize.CanReadClient(claims, (int) invoiceUpdateDto.ClientId))
        {
            return false;
        }

        return true;
    }

    public async Task<bool> CanDeleteInvoice(ClaimsPrincipal claims, int id) {
        return await IsInvoiceOwner(claims, id);
    }

    public async Task<bool> CanCreateInvoice(ClaimsPrincipal claims) {
        return await businessAuthorize.IsBusinessOwner(claims);
    }

    public async Task<bool> CanReadInvoice(ClaimsPrincipal claims, int id) {
        return await IsInvoiceOwner(claims, id);
    }

    public async Task<bool> CanListInvoices(ClaimsPrincipal claims) {
        return await businessAuthorize.IsBusinessOwner(claims);
    }

    public async Task<bool> CanSendInvoice(ClaimsPrincipal claims, int id) {
        return await IsInvoiceOwner(claims, id);
    }

    private async Task<bool> IsInvoiceOwner(ClaimsPrincipal claims, int id) {
        var businessId = await businessAuthorize.GetBusinessId(claims);

        // Tarkista että olet laskun omistaja
        if (!await invoiceStore.OwnedByBusiness(businessId, id)) {
            return false;
        }

        return true;
    }
}
```

Lisäksi InvoicesAuthorize täytyy rekisteröidä **Startup.cs** tiedostossa riippuvuusinjektiolle. Kannattaa käyttää transienttia rekisteröintiä.

#### InvoicesController.cs

`InvoicesController` vaatii lisäksi käsittelijöissä yrityksen tunnistamista varten kirjautuneen käyttäjän yritysolion. Sen voisi tehdä samaan tapaan kuin `LoggedIn` esimerkissä, eli hakemalla ensin käyttäjän ja sitten tästä yrityksen olion, mutta tämä on turhan työlästä jokaisessa actionissa. Tätät varten alla olevassa esimerkissä käytetty omaa `[RequestBusiness]` attribuuttiia jonka toteutus on tämän jälkeen.


```cs
[Authorize] // <-- Tämä attribuutti varmistaa että käyttäjä on kirjautunut
[Route("[controller]")]
public class InvoicesController : ControllerBase
{
    private readonly InvoiceService invoiceService;
    private readonly InvoicesAuthorize invoiceAuthorize;

    public InvoicesController(InvoiceService invoiceService, InvoicesAuthorize invoiceAuthorize)
    {
        this.invoiceService = invoiceService;
        this.invoiceAuthorize = invoiceAuthorize;
    }

    [HttpGet("{id}")]
    public async Task<InvoiceDto> Get(int id, [RequestBusiness] Business business) {
        if (!await invoiceAuthorize.CanReadInvoice(User, id)) {
            throw new Forbidden();
        }

        var invoice = await invoiceService.GetByBusiness(business.Id, id);
        return InvoiceDto.FromInvoice(invoice);
    }

    [HttpDelete("{id}")]
    public async Task<bool> Delete(int id, [RequestBusiness] Business business) {
        if (!await invoiceAuthorize.CanDeleteInvoice(User, id)) {
            throw new Forbidden();
        }

        var invoice = await invoiceService.GetByBusiness(business.Id, id);
        await invoiceService.Remove(invoice);
        return true;
    }

    [HttpPut("{id}")]
    public async Task<InvoiceDto> Update(
        int id, 
        [FromBody] InvoiceUpdateDto updateInvoiceDto,
        [RequestBusiness] Business business)
    {
        if (!await invoiceAuthorize.CanUpdateInvoice(User, id, updateInvoiceDto)) {
            throw new Forbidden();
        }

        var invoice = await invoiceService.GetByBusiness(business.Id, id);

        // Päivitä laskua dtosta
        updateInvoiceDto.UpdateInvoice(invoice);
        await invoiceService.Update(invoice);

        // Palauta päivitetty lasku, kierretään tietokannan kautta jotta
        // data päivittyy oikein
        return InvoiceDto.FromInvoice(
            await invoiceService.GetByBusiness(business.Id, id)
        );
    }

    [HttpPost]
    public async Task<InvoiceDto> Create([RequestBusiness] Business business) {
        if (!await invoiceAuthorize.CanCreateInvoice(User)) {
            throw new Forbidden();
        }

        // Huomaa että tässä esimerkissä en ota sisään dataa josta lasku
        // luotaisiin, sillä haluan että tätä järjestelmää käytettäessä
        // luodaan aina ensin luonnos, eli tyhjä lasku jota aletaan
        // muokkaamaan.
        var invoice = await invoiceService.Create(new Invoice() {
            BusinessId = business.Id
        });
        return InvoiceDto.FromInvoice(invoice);
    }

    [HttpGet]
    public async Task<IEnumerable<InvoiceDto>> ListLatest([RequestBusiness] Business business) {
        if (!await invoiceAuthorize.CanListInvoices(User)) {
            throw new Forbidden();
        }

        return (await invoiceService.ListLatestByBusiness(business.Id))
            .Select(t => InvoiceDto.FromInvoice(t))
            .ToList();
    }

    public class InvoiceSendError : ApiError<List<string>> {
        public InvoiceSendError(string message)
        {
            this.JsonData = new List<string> { message };
        }
    }

    [HttpPost("{id}/[action]")]
    public async Task<InvoiceDto> Send(int id, [RequestBusiness] Business business)
    {
        if (!await invoiceAuthorize.CanSendInvoice(User, id)) {
            throw new Forbidden();
        }

        var invoice = await invoiceService.GetByBusiness(business.Id, id);

        try {
            var sentInvoice = await invoiceService.Send(invoice);
            return InvoiceDto.FromInvoice(sentInvoice);
        } catch (InvoiceSendException ies) {
            throw new InvoiceSendError(ies.Message);
        }
    }
}
```

#### RequestBusiness attribuutin toteutus

Tässä on toteutettu edellisen rajapinnan `RequestBusiness` attribuutti. ASP.NET Coressa voi sitoa attribuutilla parametriin arvoja. Tämän attribuutin on tarkoitus hakea kirjautuneen käyttäjän yrityksen olio (Business). Tällöin sitä ei tarvitse hakea jokaisessa actionissa erikseen.

```cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Esimerkki4.Kirjautuminen.Controllers.Dtos;
using Esimerkki4.Kirjautuminen.Models;
using Esimerkki4.Kirjautuminen.Mvc;
using Esimerkki4.Kirjautuminen.Services;
using Esimerkki4.Kirjautuminen.Stores;
using IdentityModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Esimerkki4.Kirjautuminen.Auth
{
    public class RequestBusinessModelBinder : IModelBinder
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly BusinessAuthorize businessAuthorize;

        public RequestBusinessModelBinder(UserManager<ApplicationUser> userManager, 
            BusinessAuthorize authService)
        {
            this.userManager = userManager;
            this.businessAuthorize = authService;
        }

        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            bindingContext.Result = ModelBindingResult.Success(
                await businessAuthorize.RequestBusiness(bindingContext.HttpContext.User)
            );
        }
    }

    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property, 
        AllowMultiple = false, Inherited = true)]
    public class RequestBusinessAttribute : Attribute, IBinderTypeProviderMetadata
    {
        public BindingSource BindingSource
        {
            get
            {
                return new BindingSource(
                    id: "RequestBusiness",
                    displayName: "RequestBusiness",
                    isGreedy: false,
                    isFromRequest: false);
            }
        }

        Type IBinderTypeProviderMetadata.BinderType
        {
            get
            {
                return typeof(RequestBusinessModelBinder);
            }
        }

    }
}
```

`RequestBusiness` attribuutti riittä tarkistamaan että käyttäjä on kirjautunut yrityksenä, joten sitä ei välttämättä tarvitse tarkistaa enää erikseen, mutta yllä olevissa esimerkeissä se on tarkistettu.

Attribuuteilla voi joissain tilanteessa parantaa rajapinnan yleisyyttä, esimerkiksi jos vastaavasti tekisi `ClaimsPrincipal` palauttavan attribuutin, niin yhdenkään ohjaimen ei tarvisi enää periytyä `ControllerBase` luokasta. Tällä tavalla voi tehdä huomattavan yleisen rajapinnan, joka ei riipu HTTP protokollasta vaan sitä voisi käyttää esimerkiksi gRPC yhteensopivana soketin ylitse.

Myös resurssipohjaiset tarkistimet voisi toteuttaa omalla attribuutilla, esimerkiksi `[EnsureUser("CanUpdateInvoice")]` joka laitettaisiin käsittelijäkohtaisesti. Tämä helpottaisi tarkistimein dokumentointia sekä vähentäisi ohjaimen riippuvuuksia. Tässä esimerkissä ei kuitenkaan toteuteta omia attribuutteja tätä varten.

Esimerkkiohjelman arkkitehtuuri ilmenee suhteellisen hyvin seuraavasta kaaviosta, josta on poistettu juuriluokat, DTO:t sekä MVC helperit:

![API](api4-luokat.png)

Tästä näkee riippuvuudet suhteellisen hyvin, eli storeja käytetään vain servicejen toimesta eikä esimerkiksi ohjaimien toimesta. Auhtorisointiin liittyviä käytetään vain ohjaimista. Nuolenpäät merkitsevät riippuvuussuhdetta, ja näistä voi päätellä että esimerkiksi ohjaimet voi tuhota ilman että se vaikuttaa muuhun ohjelmistoon.

Esimerkkiohjelman rajapinta näyttää tältä:

![API](swagger-api4.png)

Swagger yksinään ei ole riittävä apu frontendin kehittäjälle, sen takia seuraavassa esimerkissä generoidaan vielä tälle tyyppiturvallinen SDK TypeScriptille jotta sitä voi käyttää helposti esim React/Angular ohjelmassa.


## TypeScript/JavaScript SDK:n generointi ApiExplorer kirjastolla

Samaan tapaan kuin Swagger osaa generoida rajapintatesterin voidaan generoida oma SDK. Itse olen toteuttanut tämän omana luokkana joka generoi tarpeellisen tyyppiturvallisen TypeScript rangan rajapinnalle. SDK:n generoimalla voidaan toteuttaa 

