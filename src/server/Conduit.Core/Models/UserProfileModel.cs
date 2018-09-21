namespace Conduit.Core.Models
{
    public class UserProfileModel
    {
        public string Id { get; set; }

        public string Username { get; set; }

        public string Image { get; set; }

        public string Bio { get; set; }

        public bool Following { get; set; }
    }
}
