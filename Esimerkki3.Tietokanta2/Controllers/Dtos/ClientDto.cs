using System.ComponentModel.DataAnnotations;
using Esimerkki3.Tietokanta2.Models;

namespace Esimerkki3.Tietokanta2.Controllers.Dtos
{
    public class ClientDto
    {
        public int Id { get; set; }

        [MinLength(3)]
        public string Name { get; set; } = "";
        public string Address { get; set; } = "";
        public string City { get; set; } = "";
        public string PostCode { get; set; } = "";

        [EmailAddress]
        public string Email { get; set; } = "";

        [Phone]
        public string PhoneNumber { get; set; } = "";

        public static ClientDto FromClient(Client client) {
            if (client == null) {
                return null;
            }
            return new ClientDto() {
                Email = client.Email,
                Address = client.Address,
                City = client.City,
                Id = client.Id,
                Name = client.Name,
                PhoneNumber = client.PhoneNumber,
                PostCode = client.PostCode
            };
        }

        public Client UpdateClient(Client client) {
            client.Name = Name;
            client.Address = Address;
            client.City = City;
            client.PostCode = PostCode;
            client.Email = Email;
            client.PhoneNumber = PhoneNumber;
            return client;
        }
    }
}