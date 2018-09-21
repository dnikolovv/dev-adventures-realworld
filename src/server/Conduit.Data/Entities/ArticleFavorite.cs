namespace Conduit.Data.Entities
{
    public class ArticleFavorite
    {
        public int ArticleId { get; set; }

        public string UserId { get; set; }

        public Article Article { get; set; }

        public User User { get; set; }
    }
}
