using Conduit.Core;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Conduit.Api.Controllers
{
    [Route("api/[controller]")]
    public class ApiController : Controller
    {
        protected string CurrentUserId => User.FindFirstValue(JwtRegisteredClaimNames.Sub);

        protected IActionResult Error(Error error) =>
            new BadRequestObjectResult(error);

        protected IActionResult NotFound(Error error) =>
            new NotFoundObjectResult(error);
    }
}
