using AutoMapper;
using Conduit.Business.Extensions;
using Conduit.Core;
using Conduit.Core.Models;
using Conduit.Core.Services;
using Conduit.Data.Entities;
using Conduit.Data.EntityFramework;
using Microsoft.EntityFrameworkCore;
using Optional;
using Optional.Async;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Conduit.Business.Services
{
    public class ArticleCommentsService : ArticlesService, IArticleCommentsService
    {
        public ArticleCommentsService(ApplicationDbContext dbContext, IMapper mapper)
            : base(dbContext, mapper)
        {
        }

        public Task<Option<CommentModel, Error>> CreateComment(string authorId, string articleSlug, CreateCommentModel commentModel) =>
            GetUserByIdOrError(authorId).FlatMapAsync(user =>
            GetArticleBySlug(articleSlug)
                .MapAsync(async article =>
                {
                    var comment = Mapper.Map<Comment>(commentModel);

                    comment.AuthorId = user.Id;
                    comment.ArticleId = article.Id;
                    comment.CreatedAt = DateTime.UtcNow;
                    comment.UpdatedAt = DateTime.UtcNow;

                    DbContext.Comments.Add(comment);

                    await DbContext.SaveChangesAsync();
                    await DbContext.Entry(comment).Reference(c => c.Author).LoadAsync();

                    return ToCommentModel(user, comment);
                }));

        public Task<Option<int, Error>> DeleteComment(string deletingUserId, string articleSlug, int commentId) =>
            GetUserByIdOrError(deletingUserId).FlatMapAsync(user =>
            GetArticleBySlug(articleSlug).FlatMapAsync(_ =>
            GetCommentById(commentId)
                .FilterAsync(async comment => comment.AuthorId == user.Id, "You must be the author of the comment in order to delete it.")
                .MapAsync(async comment =>
                {
                    DbContext.Remove(comment);
                    await DbContext.SaveChangesAsync();
                    return comment.Id;
                })));

        public Task<Option<CommentModel[], Error>> GetCommentsForArticle(Option<string> viewingUserId, string articleSlug) =>
            GetArticleBySlug(articleSlug).MapAsync(article =>
            GetUserById(viewingUserId)
                .MatchAsync(
                    async u => ToCommentModels(u, article?.Comments?.ToArray()),
                    async () => article.Comments.Select(Mapper.Map<CommentModel>).ToArray()));

        private CommentModel ToCommentModel(User viewingUser, Comment comment) =>
            comment != null ?
            ToCommentModels(viewingUser, comment).First() :
            null;

        private CommentModel[] ToCommentModels(User viewingUser, params Comment[] comments) =>
            comments?.Select(c =>
            {
                var model = Mapper.Map<CommentModel>(c);

                if (model.Author != null)
                {
                    model.Author.Following = IsFollowingAuthor(viewingUser, model.Author);
                }

                return model;
            })
            .ToArray();

        private Task<Option<Comment, Error>> GetCommentById(int commentId) =>
            DbContext
                .Comments
                .Include(c => c.Author)
                .FirstOrDefaultAsync(c => c.Id == commentId)
                .SomeNotNull<Comment, Error>($"No comment with an id of '{commentId}' was found.");
    }
}
