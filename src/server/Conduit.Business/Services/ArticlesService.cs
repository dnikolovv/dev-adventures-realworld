using AutoMapper;
using Conduit.Business.Extensions;
using Conduit.Core;
using Conduit.Core.Models;
using Conduit.Core.Services;
using Conduit.Data.Entities;
using Conduit.Data.EntityFramework;
using Microsoft.EntityFrameworkCore;
using Optional;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Conduit.Business.Services
{
    public class ArticlesService : BaseService, IArticlesService
    {
        public ArticlesService(ApplicationDbContext dbContext, IMapper mapper)
            : base (dbContext)
        {
            Mapper = mapper;
        }

        protected IQueryable<Article> AllArticlesQueryable => DbContext
            .Articles
            .AsNoTracking()
            .Include(a => a.Comments)
                .ThenInclude(c => c.Author)
            .Include(a => a.TagList)
                .ThenInclude(at => at.Tag)
            .Include(a => a.Favorites)
                .ThenInclude(af => af.User)
            .Include(a => a.Author)
                .ThenInclude(a => a.Favorites);

        protected IMapper Mapper { get; }

        public Task<Option<ArticleModel, Error>> CreateAsync(string authorId, CreateArticleModel model) =>
            GetUserByIdOrError(authorId)
                .MapAsync(async author =>
                {
                    var article = Mapper.Map<Article>(model);

                    article.TagList = FromTagListToDbCollection(article, model.TagList);
                    article.AuthorId = author.Id;
                    article.CreatedAt = DateTime.UtcNow;
                    article.UpdatedAt = DateTime.UtcNow;

                    DbContext.Articles.Add(article);

                    await DbContext.SaveChangesAsync();

                    return Mapper.Map<ArticleModel>(article);
                });

        public async Task<ArticleModel[]> GetAsync(Option<string> viewingUserId, GetArticlesModel request)
        {
            var articlesQueryable = AllArticlesQueryable;

            request.Tag.MatchSome(tagFilter =>
                articlesQueryable = articlesQueryable.Where(a => a.TagList.Any(at => at.Tag.Name == tagFilter)));

            request.Author.MatchSome(authorFilter =>
                articlesQueryable = articlesQueryable.Where(a => a.Author.UserName == authorFilter));

            request.Favorited.MatchSome(favoritedFilter =>
                articlesQueryable = articlesQueryable.Where(a => a.Favorites.Any(af => af.User.UserName == favoritedFilter)));

            articlesQueryable = articlesQueryable
                .Skip(request.Offset)
                .Take(request.Limit);

            var articles = await articlesQueryable.ToArrayAsync();

            return await ToArticleModelsAsync(viewingUserId, articles);
        }

        public Task<Option<ArticleModel, Error>> UpdateAsync(string updatingUserId, string articleSlug, UpdateArticleModel model) =>
            GetUserByIdOrError(updatingUserId).FlatMapAsync(user =>
            GetArticleBySlug(articleSlug)
                .FilterAsync(async a => a.Author.Id == user.Id, "You must be the author in order to update an article.")
                .FlatMapAsync(async article =>
                {
                    Mapper.Map(model, article);

                    article.TagList = FromTagListToDbCollection(article, model.TagList);
                    article.UpdatedAt = DateTime.UtcNow;
                    DbContext.Update(article);

                    await DbContext.SaveChangesAsync();

                    return await GetBySlugAsync(user.Id.Some(), articleSlug);
                }));

        public Task<Option<ArticleModel, Error>> GetBySlugAsync(Option<string> viewingUserId, string slug) =>
            GetArticleBySlug(slug)
                .MapAsync(article =>
                    ToArticleModelAsync(viewingUserId, article));

        public Task<Option<ArticleModel[], Error>> GetFeed(string viewingUserId, int limit = 20, int offset = 0) =>
            GetUserByIdOrError(viewingUserId)
                .MapAsync(async viewingUser =>
                {
                    var followedUserIds = viewingUser
                        .Following
                        .Select(f => f.FollowingId)
                        .ToArray();

                    var articles = await AllArticlesQueryable
                        .Where(a => followedUserIds.Contains(a.Author.Id))
                        .Skip(offset)
                        .Take(limit)
                        .ToArrayAsync();

                    return await ToArticleModelsAsync(viewingUserId.Some(), articles);
                });

        public Task<Option<ArticleModel, Error>> FavoriteAsync(string userId, string slug) =>
            GetUserByIdOrError(userId).FlatMapAsync(user =>
            GetArticleBySlug(slug)
                .FilterAsync(async article => !article.Favorites.Any(af => af.UserId == userId), "You have already favorited this article.")
                .FlatMapAsync(async article =>
                {
                    var articleFavorite = new ArticleFavorite
                    {
                        ArticleId = article.Id,
                        UserId = userId
                    };

                    DbContext.ArticleFavorites.Add(articleFavorite);

                    await DbContext.SaveChangesAsync();

                    return await GetBySlugAsync(user.Id.Some(), slug);
                }));

        public Task<Option<ArticleModel, Error>> UnfavoriteAsync(string userId, string slug) =>
            GetUserByIdOrError(userId).FlatMapAsync(user =>
            GetArticleBySlug(slug)
                .FilterAsync(async a => a.Favorites.Any(af => af.UserId == userId), "You have not favorited this article.")
                .FlatMapAsync(async article =>
                {
                    var articleFavorite = article
                        .Favorites
                        .First(af => af.UserId == userId);

                    DbContext.Remove(articleFavorite);

                    await DbContext.SaveChangesAsync();

                    return await GetBySlugAsync(user.Id.Some(), slug);
                }));

        public Task<Option<int, Error>> DeleteAsync(string deletingUserId, string slug) =>
            GetUserByIdOrError(deletingUserId).FlatMapAsync(user =>
            GetArticleBySlug(slug)
                .FilterAsync(async a => a.Author.Id == user.Id, "You must be the author in order to delete an article.")
                .MapAsync(async article =>
                {
                    DbContext.Remove(article);
                    await DbContext.SaveChangesAsync();
                    return article.Id;
                }));

        protected static bool IsFollowingAuthor(User viewingUser, UserProfileModel author) =>
            viewingUser != null &&
            author != null &&
            (viewingUser.Following?.Any(f => f.FollowingId == author.Id) ?? false);

        protected Task<Option<Article, Error>> GetArticleBySlug(string slug) =>
            slug.SomeWhen<string, Error>(s => !string.IsNullOrEmpty(s), "The slug must not be empty.")
                .Map(s => s.ToLower())
                .FlatMapAsync(async s =>
                    await AllArticlesQueryable
                        .OrderByDescending(a => a.CreatedAt)
                        .FirstOrDefaultAsync(a => a.Slug.ToLower() == slug.ToLower())
                        .SomeNotNull<Article, Error>($"No article with slug '{slug}' was found."));

        private List<ArticleTag> FromTagListToDbCollection(Article article, ICollection<string> tagList)
        {
            if (!tagList?.Any() ?? false)
                return new List<ArticleTag>();

            tagList = tagList.Select(s => s.ToLower()).ToList();

            // If any tags already exist, we want to attach them to the article and not create new ones
            var tagsInDb = DbContext
                .Tags
                .Where(t => tagList.Contains(t.Name.ToLower()))
                .ToDictionary(t => t.Name);

            return tagList.Select(tagName =>
            {
                if (tagsInDb.ContainsKey(tagName))
                    return new ArticleTag { ArticleId = article.Id, TagId = tagsInDb[tagName].Id };

                var tagToCreate = new Tag { Name = tagName };

                DbContext.Tags.Add(tagToCreate);

                return new ArticleTag
                {
                    ArticleId = article.Id,
                    TagId = tagToCreate.Id
                };
            }).ToList();
        }

        private async Task<ArticleModel> ToArticleModelAsync(Option<string> viewingUserId, Article article) =>
            (await ToArticleModelsAsync(viewingUserId, article)).FirstOrDefault();

        private Task<ArticleModel[]> ToArticleModelsAsync(Option<string> viewingUserId, params Article[] articles)
        {
            var models = articles
                .OrderByDescending(a => a.CreatedAt)
                .Select(Mapper.Map<ArticleModel>)
                .ToArray();

            return SetFollowingAndFavorited(viewingUserId, models);
        }

        private async Task<ArticleModel> SetFollowingAndFavorited(Option<string> viewingUserId, ArticleModel article) =>
            article != null ?
            (await SetFollowingAndFavorited(viewingUserId, new[] { article })).First() :
            null;

        private Task<ArticleModel[]> SetFollowingAndFavorited(Option<string> viewingUserId, params ArticleModel[] articles) =>
            GetUserById(viewingUserId)
                .MatchAsync(
                    async user => articles.Select(a => SetFollowingAndFavorited(user, a)).ToArray(),
                    async () => articles);

        private ArticleModel SetFollowingAndFavorited(User user, ArticleModel article)
        {
            if (article?.Author != null)
            {
                article.Author.Following = IsFollowingAuthor(user, article.Author);
            }

            article.Favorited = user?.Favorites?.Any(af => af.ArticleId == article.Id) ?? false;

            return article;
        }
    }
}