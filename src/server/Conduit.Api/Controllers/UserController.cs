using Conduit.Api.Requests;
using Conduit.Core;
using Conduit.Core.Models;
using Conduit.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Conduit.Api.Controllers
{
    [Authorize]
    public class UserController : ApiController
    {
        private readonly IUsersService _usersService;

        public UserController(IUsersService usersService)
        {
            _usersService = usersService;
        }

        /// <summary>
        /// Returns the current logged in user.
        /// </summary>
        /// <returns>The current logged in user.</returns>
        /// <response code="200">Returns the currently logged in user.</response>
        /// <response code="404">No user was found (this would mean that something is wrong with the claims).</response>
        [HttpGet]
        [ProducesResponseType(typeof(UserModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetCurrentUser() =>
            (await _usersService.GetByIdAsync(CurrentUserId))
            .Match<IActionResult>(user => Ok(new { user }), NotFound);

        /// <summary>
        /// Updates the currently logged in user.
        /// </summary>
        /// <param name="request">The update request.</param>
        /// <returns>The updated user profile or an error.</returns>
        /// <response code="200">Returns the updated user profile.</response>
        /// <response code="400">Returns the error/s that occurred while trying to update.</response>
        [HttpPut]
        [ProducesResponseType(typeof(UserModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateCurrentUser([FromBody] UpdateUserRequest request) =>
            (await _usersService.UpdateAsync(CurrentUserId, request.User))
            .Match<IActionResult>(user => Ok(new { user }), Error);
    }
}
