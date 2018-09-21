using Conduit.Core.Models;
using Optional;
using System.Threading.Tasks;

namespace Conduit.Core.Services
{
    public interface IArticleCommentsService
    {
        Task<Option<CommentModel[], Error>> GetCommentsForArticle(Option<string> viewingUserId, string articleSlug);

        Task<Option<CommentModel, Error>> CreateComment(string authorId, string articleSlug, CreateCommentModel comment);

        Task<Option<int, Error>> DeleteComment(string deletingUserId, string articleSlug, int commentId);
    }
}
