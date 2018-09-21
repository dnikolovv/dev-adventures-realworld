using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Conduit.Core.Models
{
    public class CreateArticleModel
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string Body { get; set; }

        public ICollection<string> TagList { get; set; } = new List<string>();
    }
}
