using System.ComponentModel.DataAnnotations;

namespace Esimerkki3.Tietokanta2.Controllers.Dtos
{
    public class ClientDto
    {
        public int Id { get; set; }

        [MinLength(3)]
        public string Name { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string PostCode { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        [Phone]
        public string PhoneNumber { get; set; }
    }
}