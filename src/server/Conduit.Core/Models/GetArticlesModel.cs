using Optional;

namespace Conduit.Core.Models
{
    public class GetArticlesModel
    {
        public Option<string> Tag { get; set; }

        public Option<string> Author { get; set; }

        public Option<string> Favorited { get; set; }

        public int Limit { get; set; } = 20;

        public int Offset { get; set; } = 0;
    }
}
