using System;

namespace Conduit.Core.Models
{
    public class CommentModel
    {
        public int Id { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public string Body { get; set; }

        public UserProfileModel Author { get; set; }
    }
}
