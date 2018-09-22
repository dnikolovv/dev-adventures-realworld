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
    public class ProfilesController : ApiController
    {
        private readonly IProfilesService _profilesService;

        public ProfilesController(IProfilesService profilesService)
        {
            _profilesService = profilesService;
        }

        /// <summary>
        /// Retreives a user's profile by username.
        /// </summary>
        /// <param name="username">The username to look for.</param>
        /// <returns>A user profile or not found.</returns>
        /// <response code="200">Returns the user's profile.</response>
        /// <response code="404">No user with tha given username was found.</response>
        [HttpGet("{username}")]
        [ProducesResponseType(typeof(UserProfileModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Get(string username) =>
            (await _profilesService.ViewProfileAsync(CurrentUserId.SomeNotNull(), username))
            .Match<IActionResult>(profile => Ok(new { profile }), Error);

        /// <summary>
        /// Follows a user.
        /// </summary>
        /// <param name="username">The user to follow.</param>
        /// <returns>The followed user's profile or an error.</returns>
        /// <response code="200">Successfully followed the given user.</response>
        /// <response code="400">An eror occurred.</response>
        [HttpPost("{username}/follow")]
        [ProducesResponseType(typeof(UserProfileModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Follow(string username) =>
            (await _profilesService.FollowAsync(CurrentUserId, username))
            .Match<IActionResult>(profile => Ok(new { profile }), Error);

        /// <summary>
        /// Unfollows a user.
        /// </summary>
        /// <param name="username">The user to unfollow.</param>
        /// <returns>The unfollowed user's profile or an error.</returns>
        /// <response code="200">Successfully unfollowed the given user.</response>
        /// <response code="400">An eror occurred.</response>
        [HttpDelete("{username}/follow")]
        [ProducesResponseType(typeof(UserProfileModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Unfollow(string username) =>
            (await _profilesService.UnfollowAsync(CurrentUserId, username))
            .Match<IActionResult>(profile => Ok(new { profile }), Error);
    }
}
