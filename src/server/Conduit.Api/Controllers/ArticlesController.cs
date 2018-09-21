using Conduit.Api.Requests;
using Conduit.Core;
using Conduit.Core.Models;
using Conduit.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Optional;
using System.Threading.Tasks;

namespace Conduit.Api.Controllers
{
    [Authorize]
    public class ArticlesController : ApiController
    {
        private readonly IArticlesService _articlesService;
        private readonly IArticleCommentsService _commentsService;

        public ArticlesController(IArticlesService articlesService, IArticleCommentsService commentsService)
        {
            _articlesService = articlesService;
            _commentsService = commentsService;
        }

        /// <summary>
        /// Retrieves a list of articles by various filters.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>A list of articles.</returns>
        /// <response code="200">Successfully fetched the requested articles.</response>
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ArticleModel[]), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Get(GetArticlesModel request)
        {
            var articles = await _articlesService.GetAsync(CurrentUserId.SomeNotNull(), request);
            return Ok(new { articles, articlesCount = articles.Length });
        }

        /// <summary>
        /// Retrieves an article by its slug.
        /// </summary>
        /// <param name="slug">The slug.</param>
        /// <returns>An article or not found.</returns>
        /// <response code="200">An article.</response>
        /// <response code="404">Could not find the given article.</response>
        [HttpGet("{slug}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ArticleModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetBySlug(string slug) =>
            (await _articlesService.GetBySlugAsync(CurrentUserId.SomeNotNull(), slug))
            .Match<IActionResult>(article => Ok(new { article }), NotFound);

        /// <summary>
        /// Retrieves the logged in user's feed.
        /// </summary>
        /// <param name="limit">Limit of shown articles.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>A list of articles.</returns>
        /// <response code="200">The current users' feed.</response>
        /// <response code="404">If the id of the current user was not found (most likely a problem with the JWT claims).</response>
        [HttpGet("feed")]
        [ProducesResponseType(typeof(ArticleModel[]), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetFeed([FromQuery] int limit = 20, int offset = 0) =>
            (await _articlesService.GetFeed(CurrentUserId, limit, offset))
            .Match<IActionResult>(articles => Ok(new { articles, articlesCount = articles.Length }), NotFound);

        /// <summary>
        /// Creates an article.
        /// </summary>
        /// <param name="request">The article data.</param>
        /// <returns>An article model or an error.</returns>
        /// <response code="201">An article was successfully created and an article model was returned.</response>
        /// <response code="400">An error occurred.</response>
        [HttpPost]
        [ProducesResponseType(typeof(ArticleModel), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Post([FromBody] CreateArticleRequest request) =>
            (await _articlesService.CreateAsync(CurrentUserId, request.Article))
            .Match<IActionResult>(article => CreatedAtAction(nameof(Post), new { article }), Error);

        /// <summary>
        /// Updates an article.
        /// </summary>
        /// <param name="slug">The article slug.</param>
        /// <param name="request">The article data.</param>
        /// <returns>An article model or an error.</returns>
        /// <response code="200">An article was successfully updated and an article model was returned.</response>
        /// <response code="400">An error occurred.</response>
        [HttpPut("{slug}")]
        [ProducesResponseType(typeof(ArticleModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Put(string slug, [FromBody] UpdateArticleRequest request) =>
            (await _articlesService.UpdateAsync(CurrentUserId, slug, request.Article))
            .Match<IActionResult>(article => Ok(new { article }), Error);

        /// <summary>
        /// Deletes an article.
        /// </summary>
        /// <param name="slug">The article slug.</param>
        /// <returns>The deleted article id or an error.</returns>
        /// <response code="204">Successfully deleted the article.</response>
        /// <response code="400">An error occurred.</response>
        [HttpDelete("{slug}")]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete(string slug) =>
            (await _articlesService.DeleteAsync(CurrentUserId, slug))
            .Match<IActionResult>(_ => NoContent(), Error);

        /// <summary>
        /// Favorites an article.
        /// </summary>
        /// <param name="slug">The article slug.</param>
        /// <returns>Ok or an error.</returns>
        /// <response code="200">Successfully favorited the given article.</response>
        [HttpPost("{slug}/favorite")]
        [ProducesResponseType(typeof(ArticleModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Favorite(string slug) =>
            (await _articlesService.FavoriteAsync(CurrentUserId, slug))
            .Match<IActionResult>(article => Ok(new { article }), Error);

        /// <summary>
        /// Removes an article favorite.
        /// </summary>
        /// <param name="slug">The article slug.</param>
        /// <returns>Ok or an error.</returns>
        /// <response code="200">Successfully unfavorited the given article.</response>
        [HttpDelete("{slug}/favorite")]
        [ProducesResponseType(typeof(ArticleModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Unfavorite(string slug) =>
            (await _articlesService.UnfavoriteAsync(CurrentUserId, slug))
            .Match<IActionResult>(article => Ok(new { article }), Error);

        /// <summary>
        /// Gets all comments for a given article.
        /// </summary>
        /// <param name="slug">The article slug.</param>
        /// <returns>A list of comments.</returns>
        /// <response code="200">Got a list of comments for the given article.</response>
        [HttpGet("{slug}/comments")]
        [ProducesResponseType(typeof(CommentModel[]), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetComments(string slug) =>
            (await _commentsService.GetCommentsForArticle(CurrentUserId.SomeNotNull(), slug))
            .Match<IActionResult>(comments => Ok(new { comments }), Error);

        /// <summary>
        /// Creates a comment.
        /// </summary>
        /// <param name="slug">The article slug.</param>
        /// <param name="request">The comment content.</param>
        /// <returns>The created comment.</returns>
        /// <response code="201">A comment was created.</response>
        [HttpPost("{slug}/comments")]
        [ProducesResponseType(typeof(CommentModel[]), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateComment(string slug, [FromBody] CreateCommentRequest request) =>
            (await _commentsService.CreateComment(CurrentUserId, slug, request.Comment))
            .Match<IActionResult>(comment => CreatedAtAction(nameof(CreateComment), new { comment }), Error);

        /// <summary>
        /// Deletes a comment.
        /// </summary>
        /// <param name="slug">The article slug.</param>
        /// <param name="commentId">The comment id.</param>
        /// <returns>No content result.</returns>
        /// <response code="204">The comment was successfully deleted.</response>
        [HttpDelete("{slug}/comments/{commentId:int}")]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteComment(string slug, int commentId) =>
            (await _commentsService.DeleteComment(CurrentUserId, slug, commentId))
            .Match<IActionResult>(_ => NoContent(), Error);
    }
}
