using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace Conduit.Data.Entities
{
    public class User : IdentityUser<string>
    {
        public string Bio { get; set; }

        public string Image { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTime RegistrationDate { get; set; }

        public List<Comment> Comments { get; set; } = new List<Comment>();

        public List<ArticleFavorite> Favorites { get; set; } = new List<ArticleFavorite>();

        public List<FollowedUser> Following { get; set; } = new List<FollowedUser>();

        public List<FollowedUser> Followers { get; set; } = new List<FollowedUser>();
    }
}
