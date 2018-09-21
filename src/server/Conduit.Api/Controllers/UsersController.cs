using Conduit.Api.Requests;
using Conduit.Core;
using Conduit.Core.Models;
using Conduit.Core.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Conduit.Api.Controllers
{
    public class UsersController : ApiController
    {
        private readonly IUsersService _usersService;

        public UsersController(IUsersService usersService)
        {
            _usersService = usersService;
        }

        /// <summary>
        /// Login.
        /// </summary>
        /// <param name="request">The credentials.</param>
        /// <returns>A JWT token.</returns>
        /// <response code="200">If the credentials have a match.</response>
        /// <response code="400">If the credentials don't match/don't meet the requirements.</response>
        [HttpPost("login")]
        [ProducesResponseType(typeof(UserModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Login([FromBody] LoginUserRequest request) =>
            (await _usersService.LoginAsync(request.User))
            .Match(user => Ok(new { user }), Error);

        /// <summary>
        /// Register.
        /// </summary>
        /// <param name="request">The user model.</param>
        /// <returns>A user model.</returns>
        /// <response code="201">A user was created.</response>
        /// <response code="400">Invalid input.</response>
        [HttpPost]
        [ProducesResponseType(typeof(UserModel), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] RegisterUserRequest request) =>
            (await _usersService.RegisterAsync(request.User))
            .Match(user => CreatedAtAction(nameof(Register), new { user }), Error);
    }
}