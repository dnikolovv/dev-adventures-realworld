namespace Conduit.Data.Entities
{
    public class ArticleTag
    {
        public int ArticleId { get; set; }

        public int TagId { get; set; }

        public Article Article { get; set; }

        public Tag Tag { get; set; }
    }
}
