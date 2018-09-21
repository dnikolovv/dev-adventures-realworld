using System.Collections.Generic;

namespace Conduit.Core.Models
{
    public class UpdateArticleModel
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public string Body { get; set; }

        public ICollection<string> TagList { get; set; } = new List<string>();
    }
}
