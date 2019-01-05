using Microsoft.AspNetCore.Mvc;

namespace Conduit.Api.Controllers
{
    public class HealthController : ApiController
    {
        [HttpGet]
        public IActionResult Health() => Ok();
    }
}
