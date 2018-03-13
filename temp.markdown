
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
