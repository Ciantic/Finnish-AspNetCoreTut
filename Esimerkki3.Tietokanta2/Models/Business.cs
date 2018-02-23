namespace Esimerkki3.Tietokanta2.Models
{
    public class Business
    {
        public int Id { get; set; }
        public string Title { get; set; }

        // Suora viittaus
        public int OwnerApplicationUserId { get; set; }
        public ApplicationUser OwnerApplicationUser { get; set; }
    }
}