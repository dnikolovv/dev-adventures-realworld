using System;
using System.ComponentModel.DataAnnotations;

namespace Conduit.Data.Entities
{
    public class Comment
    {
        public int Id { get; set; }

        [Required]
        public string Body { get; set; }

        public string AuthorId { get; set; }

        public int ArticleId { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public User Author { get; set; }

        public Article Article { get; set; }
    }
}
