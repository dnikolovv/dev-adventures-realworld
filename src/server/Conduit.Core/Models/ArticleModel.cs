using System;

namespace Conduit.Core.Models
{
    public class ArticleModel
    {
        public int Id { get; set; }

        public string Slug { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Body { get; set; }

        public string[] TagList { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public bool Favorited { get; set; }

        public int FavoritesCount { get; set; }

        public UserProfileModel Author { get; set; }
    }
}
