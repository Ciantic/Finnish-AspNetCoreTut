# C# ja ASP.NET Core 2.0

ASP.NET Core on Microsoftin tekemä web-palvelinohjelmointiin tarkoitettu ohjelmistokehys. Ensimmäinen versio tuli 2016, ja uusin versio tällä hetkellä on 2.0 joka on julkaistu 2017 elokuussa. ASP.NETistä on vanhempi versio jota on kehitetty vuodesta 2002 mutta ASP.NET Core on kokonaan uudelleenkirjoitettu versio joka ei ole taaksepäin yhteensopiva.

Samalla Microsoft julkaisi .NET Core ohljelmointiympäristön ja työkalut jotka toimivat useassa käyttöjärjestelmässä toisin kuin vanhemmat C# .NET Framework työkalut. .NET Corelle suunnattua ohjelmaa voidaan ajaa esimerkiksi pienissä Linux virtuaalikoneissa, macOS:llä tai Windowsilla.

ASP.NET Core sisältää useita kirjastoja, joista käsittelen tärkeimpiä:

* MVC - Pääkirjasto jolla voidaan toteuttaa mm. REST rajapinnat, sisältää useita kirjastoja sisällään.
* Entity Framework Core - Microsoftin tekemä tietokantakirjasto ja ORM.
* Identity Core - Kirjautumisjärjestelmän ja käyttäjien hallintaa.

Asennan myös ohjelmaan [Swagger REST-rajapintatesterin](https://swagger.io/).

Seuraavissa kohdissa esimerkkinä rakennettu ohjelma toimii kaikilla .NET Core ympäristöillä.

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

**Huomio!** Aseta ympäristömuuttuja `ASPNETCORE_ENVIRONMENT=Development` jotta dotnet käynnistyy development tilassa ja aja komento uudestaan. Ohjelman käynnistyessä pitäisi näkyä `Hosting environment: Development` kun ohjelma ajetaan development tilassa. Ympäristömuuttuja kannattaa asettaa globaalisti, yleensä vasta tuotannossa jätetään tämä pois.

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

### Rajapinnan määrittely Routing, Controller ja action

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

### Models/Models.cs tietokannan taulujen määrittely

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

Tietokantamalli luodaan siis tekemällä normaaleja luokkia, EF Core luo näistä tietyin konventioin tietokantataulut. `Int` tyyppinen `Id` kenttä on `primary key` ja auto increment, viittauskentät toisiin tauluihin on nimetty `ToinenLuokkaId` eli luokannimi johon viitataan ja Id perään, näistä tulee `foreign key`. Konventioihin voi vaikutaa `DbContext` luokalla.

### AppDbContext.cs tietokannan käsittelyluokka

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

### Startup.cs tietokantaan yhdistäminen ja testidata

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

    // Configure on synkroninen, joten tässä pitää odotella
    CreateTestData(dbContext).GetAwaiter().GetResult();
}
```

Sekä oma metodi tietokannan luomista varten, tässä käsketään ohjelmaa luomaan tietokanta aina uudestaan, lisäksi tässä luodaan testidataa:

```cs

private async Task CreateTestData(AppDbContext appDbContext) {
    // Tuhoaa ja poistaa tietokannan joka kerta
    await appDbContext.Database.EnsureDeletedAsync();
    await appDbContext.Database.EnsureCreatedAsync();

    // Luo testidata development tilalle
    var acmeUser =  new ApplicationUser() {
        Email = "testi@example.com",
        UserName = "testi@example.com",
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

### InvoicesController.cs

Luodaan nyt esimerkkinä `InvoicesController` joka toimii rajapintana laskujen luomiselle ja hallitsemiselle.

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

        public class InvoiceCreateDto {
            // Täällä määritellään kentät jota API:n kautta saa luoda
        }

        [HttpPost] 
        public async Task<bool> Create([FromBody] InvoiceCreateDto invoiceCreateDto)
        {   
            // Tänne voisi toteuttaa laskun luonnin
            // ... 
            // dbContext.Invoice.Add(invoice);
            // await dbContext.SaveChangesAsync();
            return true;
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

[Voit myös tarkastella tietokannan perusesimerkkin ohjelmakoodeja: Esimerkki2.Tietokanta](Esimerkki2.Tietokanta/)


## Dependency Injection ohjelmakirjaston ymmärtäminen

ASP.NET Core käyttää Microsoftin tekemää Dependency Injection -kirjastoa hallitsemaan eri osien riippuvuuksia. Tarkoituksena on kirjoittaa riippuvuudet Serviceinä, ja järjestelmä hoitaa näiden luomisen automaattisesti. Samaan tapaan kuin esim. Javassa Google Guice tai PHP:ssä Laravel Service container.

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

Sekä toteutukselle luokka, tässä esimerkissä ei lähetä sähköpostia vaan se pelkästään tallennetaan `Email` tietokantatauluun, vaikka toteutus lähettäisi sähköpostin se kannattaa myös tallentaa sähköpostitauluun jotta voi helpommin seurata lähetettyjä sähköposteja tai etsiä virheitä:

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

## Tietokantaesimerkki 2. - Repository pattern, Servicet, Datan validointi

Edellisessä esimerkissä tehtiin paljon pieniä asioita oikaisten, nyt korjataan nämä ja toteutetaan jotain pysyvämpää. Pienemmissä ohjelmissa `AppDbContext` tietokantakäsittelijää voi kutsua kontrollerista suoraan, mutta SQL kyselyiden, ja business logiikan sekoittaminen kontrolleriin tekee asioista hyvin hankalaa isommissa ohjelmistoissa.

Tarkoitus on jakaa ohjelmakoodi selkeisiin osiin haasteiden eriyttämisellä (separation of concerns), kontrollerit validoivat käyttäjän datan ja kutsuvat servicejä. Servicet käyttävät storeja. Storet tallentavat ja hakevat dataa tietokannasta.

`Controller <-> Service <-> Store`

Ennen tätä siistitään muutami asioita jotka jäivät tarkoituksella puoli tiehen edellisessä esimerkissä. Ensin kannattaa modelit jakaa omiin tiedostoihinsa.

### SQL-asetukset appsettings.json tiedostoon

Tietokannan asetukset kovakoodattiin ohjelmakoodiin, siirretään ne nyt konfigurointi tiedostoon, avaa **appsettings.Development.json**, syötä tietokannan asetus sinne näin:

```json
"ConnectionStrings" : {
    "Database" : "Data Source=esimerkki.development.db;"
}
```

Nyt muokataan **Startup.cs** tiedostoa siten että edellisessä esimerkissä kovakoodattu yhteysosoite haetaan konfiguraatiotiedostota, avaa `ConfigureServices` metodi ja muokkaa se seuraavaksi:

```cs
services.AddDbContext<AppDbContext>(o => {
    o.UseSqlite(Configuration.GetConnectionString("Database"));
});
```

### Testi datalle oma luokka

Siirretään tietokannan luontia varten tehty `CreateTestData()` metodi omaan luokkaansa, luodaan ensin rajapinta `IInitDb` joka toimii perustana tietokannan alustamiselle käynnistyessä.

```cs
public interface IInitDb
{
    Task Init();
}
```

Luo luokka development tilaa varten joka toteuttaaa `IInitDb` rajapinnan:

```cs
public class InitDbDevelopment : IInitDb
{
    private readonly AppDbContext appDbContext;

    public InitDbDevelopment(AppDbContext appDbContext)
    {
        this.appDbContext = appDbContext;
    }

    private async Task CreateTestData() {
        // Siirrä edellisessä esimerkissä luomasi testidatan lisäys tänne
    }

    public async Task Init()
    {
        await CreateTestData();
    }
}
```

Esimerkkiohjelmakoodeissani on testi datan luonnille huomattavasti kehittyneempi esimerkki jossa generoin dataa testitietokantaan ohjelmallisesti. Jos haluat generoida dataa etkä kirjoittaa sitä itse se kannatta toteuttaa jotenkin vastaavasti.

Luo myös luokka tuotantotilaa varten, vaikkei sitä tässä esimerkissä käytetä:

```cs
public class InitDbProduction : IInitDb
{
    public async Task Init()
    {
        // Voit ajaa erinäköisiä toimenpiteitä prosessin käynnistyksessä
        // tässä kohti, joissain tilanteissa esim migraatioita, tarkistuksia tms.

        // Huomattavaa on että ASP.NET Core prosesseja käynnistetään
        // tarvittaessa ja niitä on usein elossa useita samaan aikaan eli
        // toiminnot joita tuotannossa tässä kohti voi ajaa on hyvin
        // rajattuja
    }
}
```

Voidakseen konditionaalisesti käyttää `InitDbProduction` tai `InitDbDevelopment` luokkaa täytyy ohjelman käynnistysympäristön tieto saada myös ConfigureServices metodissa, määritellään `IHostingEnvironment` argumentiksi `Startup` rakentajassa, eli muokataan rakentaja ensin seuraavanlaiseksi:

```cs
    public class Startup
    {

        public IConfiguration Configuration { get; }

        public IHostingEnvironment Environment { get; }
        
        public Startup(IHostingEnvironment env, IConfiguration configuration)
        {
            Configuration = configuration;
            Environment = env; // Ottaa environment tiedon paikalliseen muuttujaan
        }

        // ...
    }
```

Määritellään toteutus `IInitDb` rajapinnalle siitä riippuen kummassa tilassa ohjelma on käynnistetty. Lisätään **Startup.cs** tiedostoon poikkeus sen mukaan kummassa tilassa ohjelma on, muokkaa `ConfigureServices()` metodia ja lisää rivit:

```cs
if (Environment.IsProduction()) {
    services.AddTransient<IInitDb, InitDbProduction>();
} else if (Environment.IsDevelopment()) {
    services.AddTransient<IInitDb, InitDbDevelopment>();
}
```

Muuta testidatan kutsu **Startup.cs** tiedoston `Configure()` metodissa seuraavaksi:

```cs
using (var scoped = app.ApplicationServices.CreateScope()) {
    scoped.ServiceProvider.GetRequiredService<IInitDb>()
        .Init().GetAwaiter().GetResult();
}
```

Tämä hakee `IInitDb` luokan toteutuksen ja kutsuu sen `Init()` metodia.


### Repository pattern (Stores)

Tietokannan käsittely kannattaa siirtää omiin luokkiinsa, käytän tässä repository patternin kaltaista suunnittelumallia, luokat ovat nimetty esim. `InvoiceStore`, `BusinessStore`, `ClientStore`, store jälkiliitettä käytetään esimerkiksi Identity kirjaston luokissa.

#### `InvoiceStore.cs`

Tässä on esimerkkinä `InvoiceStore` joka tallentaa, lukee ja hakee ohjelman laskuja, voit katsoa muut Storet esimerkkikoodeista. En käytä tässä mitään yleistystä, perintää, tai koodin generointia CRUD toimintojen tekemiseksi. 

```cs
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Esimerkki3.Tietokanta2.Db;
using Esimerkki3.Tietokanta2.Models;
using Microsoft.EntityFrameworkCore;

namespace Esimerkki3.Tietokanta2.Stores {
    public class InvoiceStore
    {
        private readonly AppDbContext dbContext;

        public InvoiceStore(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task Update(Invoice invoice) {
            dbContext.Invoice.Update(invoice);
            await dbContext.SaveChangesAsync();
        }

        public async Task<Invoice> Create(Invoice invoice) {
            dbContext.Invoice.Add(invoice);
            await dbContext.SaveChangesAsync();
            return invoice;
        }

        public async Task<Invoice> Delete(Invoice invoice) {
            dbContext.Invoice.Remove(invoice);
            await dbContext.SaveChangesAsync();
            return invoice;
        }

        public async Task<Invoice> Get(int id) {
            var value = await dbContext.Invoice.FirstOrDefaultAsync(t => t.Id == id);
            if (value == null) {
                throw new NotFoundException();
            }
            return value;
        }

        public async Task<ICollection<Invoice>> ListLatestByBusiness(int businessId) {
            return await dbContext.Invoice
                .Where(t => t.BusinessId == businessId)
                .Include(t => t.InvoiceRows) // Sisällytä kyselyyn laskurivien tiedot
                .Include(t => t.Client) // Sisällytä kyselyyn asiakkaantiedot
                .OrderBy(t => t.Created)
                .ToListAsync();
        }
    }
}
```

Huomaa että tässä käytetään listauksen näyttmisessä EF Coren `Include` metodia joka palauttaessaan rivin täyttää myös rivin viittaukset arvoilla. EF Coressa ei ole vielä viitteiden laiskaa latausta vaan ne pitää itse muistaa listata jos niitä tarvitsee.

### Services layer

Tarkoitus on luoda Servicet ja toiminnot kullekkin ilmeiselle käyttötapaukselle:

* Yritykset rekisteröityvät
    * `BusinessService.Register()`
* Yritys lisää, poistaa ja muokkaa omia asiakkaitaan
    * `ClientService.Create()`
    * `ClientService.Delete()`
    * `ClientService.Update()`
    * `ClientService.List()`
* Yrityksen pitää pystyä listaamaan, näyttämään, lisäämään, tuhoamaan, muokkaamaan, ja lähettämään laskujaan
    * `InvoiceService.Create()`
    * `InvoiceService.Delete()`
    * `InvoiceService.Update()`
    * `InvoiceService.Send()`
    * `InvoiceService.Get()`
    * `InvoiceService.ListLatest()`
* Järjestelmän pitää pystyä lähettämään ilmoituksia
    * `NotificationService.SendRegisterBusiness()` - yrityksen rekisteröitymissähköposti
    * `NotificationService.SendForgotPassword()` - unohditko salasanasi
    
#### `InvoiceServices.cs`

Luodaan Servicet kullekkin 

## MVC pääkirjasto

Middlewaret, Filtterit

## Identity Core käyttäjä- ja roolienhallintakirjasto

## Rajapinta SDK:n generointi MVC:n ApiExplorer kirjastolla

