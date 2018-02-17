using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using Esimerkki2.Tietokanta.Models;
using Microsoft.AspNetCore.Identity;

namespace Esimerkki2.Tietokanta.Db
{
    public class InitDbDevelopment : IInitDb
    {
        private readonly AppDbContext appDbContext;
        private readonly IPasswordHasher<ApplicationUser> passwordHasher;

        public InitDbDevelopment(AppDbContext appDbContext, IPasswordHasher<ApplicationUser> passwordHasher)
        {
            this.appDbContext = appDbContext;
            this.passwordHasher = passwordHasher;
        }

        private async Task CreateTestDatabase() {
            // Luo tietokannan aina uudestaan kun development tila käynnistetään
            await appDbContext.Database.EnsureDeletedAsync();
            await appDbContext.Database.EnsureCreatedAsync();
        }

        private async Task CreateTestData() {
            // Luo testidata development tilalle

            var acmeUser =  new ApplicationUser() {
                Email = "testi@example.com",
                UserName = "testi@example.com",
                PasswordHash = passwordHasher.HashPassword(null, "!Pass1")
            };

            var acmeBusiness = new Business() {
                Title = "Acme Inc",
                OwnerApplicationUser = acmeUser,
            };

            var clients = new List<Client>() {
                new Client() {
                    Business = acmeBusiness,
                    Title = "Kukkaismyynti Oy",
                    Address = "Kukkaiskuja 3",
                    City = "Jyväskylä",
                    PostCode = "40100",
                    Email = "kukkaismyynti@example.com",
                    PhoneNumber = "+3585012341234"
                },
                new Client() {
                    Business = acmeBusiness,
                    Title = "Kynäkauppiaat Ry",
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

        private static Dictionary<String, int> usedEmails = new Dictionary<string, int>();        

        private string GenerateUniqueEmail(string prefix, string domain = "example.com") {
            var email = prefix + "@" + domain;
            if (usedEmails.ContainsKey(email)) {
                var num = ++usedEmails[email];
                return prefix + num + "@" + domain;
            } else {
                usedEmails[email] = 1;
            }
            return email;
        }

        private Random phoneNumberRandom = new Random(1234);

        private string GeneratePhoneNumber(string prefix = "+358") {
            return prefix + phoneNumberRandom.Next(100000,900000).ToString();
        }

        private static int fistNameIndex = 0;
        private static int lastNameIndex = 0;
        private static int genderIndex = 0;

        private (string FirstName, string LastName, string Email) GenerateHumanName()
        {
            var firstNamesFemale = new string[]
            {
                "Anna", "Emilia", "Anni", "Veera", "Iida", "Jenni", "Sara", "Laura", "Noora", "Emma", "Venla", "Hanna", "Elina", "Aino"
            };

            var firstNamesMale = new string[]
            {
                "Mikko", "Juho", "Samuel", "Eetu", "Teemu", "Otto", "Sami", "Luukas", "Tuukka", "Ville", "Santtu", "Jaakko", "Frans", "Elias"
            };

            var lastNames = new string[]
            {
                "Virtanen", "Korhonen", "Nieminen", "Mäkinen", "Mäkelä", "Hämäläinen", "Laine", "Koskinen", "Heikkinen", "Järvinen"
            };

            var firstName = "";

            // Even is female, odd is male
            if (genderIndex++ % 2 == 0)
            {
                firstName = firstNamesFemale[fistNameIndex++ % firstNamesFemale.Length];
            }
            else
            {
                firstName = firstNamesMale[fistNameIndex++ % firstNamesMale.Length];
            }

            var lastName = lastNames[lastNameIndex++ % lastNames.Length];
            var email = GenerateUniqueEmail($"{firstName.ToLower()}.{lastName.ToLower().Replace("ä", "a")}");

            return (firstName, lastName, email);
        }

        private int businessNamePart1N = 0;
        private int businessNamePart2N = 0;
        private string GenerateBusinessName() {
            var first = new string[] {
                "Acme", "Globex", "Umbrella", "Hooli", "Vehement", "Massive", "Icetech", "Kiuas"
            };
            var second = new string[] {
                "", "Oy", "Inc", "Corporation", "& Partners"
            };
            return first[businessNamePart1N++ % first.Length] + " " + second[businessNamePart2N++ % second.Length];
        }

        private int invoiceNamesN = 0;
        private string GenerateInvoiceRowName() {
            var names = new string[] {
                "A4 Paperia",
                "Kirjekuoret",
                "Kuulakärkikyniä",
                "Sähköpöytä",
                "Tietokoneen näyttö",
                "Puhelin",
                "Headset mikrofoni ja kuulokkeet",
                "Viivakoodinlukija",
                "Pehmustettu istuin"
            };

            return names[invoiceNamesN++ % names.Length];
        }

        private int citiesIndex = 0;
        private int postCodesIndex = 0;
        private int addressIndex = 0;
        private int addressNumIndex = 0;
        private (string Address, string City, string PostCode) GenerateAddress() {
            var postCodesByCities = new (string city, string[] postCodes)[] {
                ("Helsinki", new string[] { "00270", "00250", "00850", "00910", "00970" } ),
                ("Jyväskylä", new string[] { "40100", "40340", "40630", "40740", "41820" } ),
                ("Vantaa", new string[] { "01350", "01510", "01730", "01760", "01670" } ),
                ("Pori", new string[] { "28220", "28900", "28600", "28540", "38670" } ),
                ("Tampere", new string[] { "33210", "33420", "33610", "33720", "33870" } ),
                ("Oulu", new string[] { "90140", "90250", "90400", "90510", "90560" } ),
            };

            var name = new string[] {
                "Rantakatu",
                "Kirkkotie",
                "Kapearaitti",
                "Kauppakatu",
                "Lehtitie",
                "Hämeenkatu",
                "Wallininkatu",
                "Korkeavuorenkatu",
                "Urho Kekkosen katu",
            };

            var num = new string[] {
                "7",
                "14",
                "9",
                "4 as 3",
                "24B",
                "19",
                "12A",
                "392"
            };

            var pc = postCodesByCities[citiesIndex++ % postCodesByCities.Length];

            return (
                Address: name[addressIndex++ % name.Length] + " " + num[addressNumIndex++ % num.Length],
                City:  pc.city,
                PostCode: pc.postCodes[postCodesIndex++ % pc.Item2.Length]
            );
        }

        private int applicationUserIndex = 1;

        private ApplicationUser GenerateApplicationUser(string emailPrefix) {
            var email = GenerateUniqueEmail(emailPrefix);
            return new ApplicationUser() {
                Email = email,
                UserName = email,
                PasswordHash = passwordHasher.HashPassword(null, "!Pass1")
            };
        }

        private Business GenerateBusiness() {
            return new Business() {
                OwnerApplicationUser = GenerateApplicationUser("business"),
                Title = GenerateBusinessName(),                
            };
        }

        private Client GenerateClient(Business business) {
            var address = GenerateAddress();
            var name = GenerateHumanName();
            return new Client() {
                Business = business,
                Address = address.Address,
                PostCode = address.PostCode,
                City = address.City,
                Title = name.FirstName + " " + name.LastName,
                Email = name.Email,
                PhoneNumber = GeneratePhoneNumber(),
            };
        }

        Random invoiceAmountRandom = new Random(1234);
        Random invoiceQuantityRandom = new Random(1234);
        private InvoiceRow GenerateInvoiceRow(Invoice invoice) {
            return new InvoiceRow() {
                Amount = invoiceAmountRandom.Next(100, 10000) / 10,
                Created = invoice.Created,
                Invoice = invoice,
                Modified = invoice.Modified,
                Name = GenerateInvoiceRowName(),
                Quantity = invoiceQuantityRandom.Next(1, 7),
            };
        }

        private Random invoiceDateRandom = new Random(1234);
        private Random invoiceSentRandom = new Random(1234);
        private Random invoiceRowCountRandom = new Random(1234);
        private Invoice GenerateInvoice(Business business, Client client) {
            var titles = new string[] {
                "Toimistotarvikkeita",
                "Pöytätilaus",
                "Muuta tavaraa"
            };

            // Title
            var title = titles[invoiceNamesN++ % titles.Length];

            // Date between 2 and 180 days old
            var date = DateTime.Now.AddMinutes(- invoiceDateRandom.Next(2 * 60, 180 * 60 * 24));

            // One out of three invoices are sent
            var sent = invoiceSentRandom.Next(0, 2) == 1 ? (DateTime?) date.AddHours(4) : null;

            var invoice = new Invoice() {
                Title = title,
                Business = business,
                Client = client,
                Created = date,
                Modified = date,
                Sent = sent
            };

            var rows = new List<InvoiceRow>();
            for (int i = 0; i < invoiceRowCountRandom.Next(2, 7); i++)
            {   
                rows.Add(GenerateInvoiceRow(invoice));
            }

            invoice.InvoiceRows = rows;

            return invoice;
        }

        private async Task CreateTestDataByGeneration() {
            var entities = new List<object>();
            
            // Create businesses
            for (int i = 0; i < 3; i++)
            {
                var business = GenerateBusiness();
                entities.Add(business);

                // Create 5 clients for each 
                for (int j = 0; j < 5; j++)
                {
                    var client = GenerateClient(business);    
                    entities.Add(client);

                    // Create 15 invoices for each
                    for (int k = 0; k < 15; k++)
                    {
                        var invoice = GenerateInvoice(business, client);        
                        entities.Add(invoice);
                    }
                }
            }

            appDbContext.AddRange(entities);
            await appDbContext.SaveChangesAsync();
        }   

        public async Task InitDb()
        {
            await CreateTestDatabase();
            await CreateTestDataByGeneration();
            await CreateTestData();
        }
    }
}