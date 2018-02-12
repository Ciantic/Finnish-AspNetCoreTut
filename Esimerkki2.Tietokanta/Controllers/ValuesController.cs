using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Esimerkki2.Tietokanta.Controllers
{
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
}
