using Conduit.Core.Models;
using Optional;
using System.Threading.Tasks;

namespace Conduit.Core.Services
{
    public interface IArticlesService
    {
        Task<Option<int, Error>> DeleteAsync(string deletingUserId, string slug);

        Task<Option<ArticleModel, Error>> CreateAsync(string authorId, CreateArticleModel model);

        Task<Option<ArticleModel, Error>> UpdateAsync(string updatingUserId, string articleSlug, UpdateArticleModel model);

        Task<ArticleModel[]> GetAsync(Option<string> viewingUserId, GetArticlesModel request);

        Task<Option<ArticleModel, Error>> FavoriteAsync(string userId, string slug);

        Task<Option<ArticleModel, Error>> UnfavoriteAsync(string userId, string slug);

        Task<Option<ArticleModel, Error>> GetBySlugAsync(Option<string> viewingUserId, string slug);

        Task<Option<ArticleModel[], Error>> GetFeed(string viewingUserId, int limit, int offset);
    }
}