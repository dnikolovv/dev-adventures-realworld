using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Conduit.Data.Entities
{
    public class Article
    {
        public int Id { get; set; }

        public string Slug { get; set; }

        [Required]
        public string Title { get; set; }

        public string Description { get; set; }

        public string Body { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        [Required]
        public string AuthorId { get; set; }

        public User Author { get; set; }

        public int FavoritesCount => Favorites?.Count ?? 0;

        public List<Comment> Comments { get; set; } = new List<Comment>();

        public List<ArticleFavorite> Favorites { get; set; } = new List<ArticleFavorite>();

        public List<ArticleTag> TagList { get; set; } = new List<ArticleTag>();
    }
}
