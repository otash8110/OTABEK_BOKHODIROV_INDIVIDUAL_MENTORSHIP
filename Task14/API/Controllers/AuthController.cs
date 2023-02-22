using API.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly ITokenService tokenService;

        public AuthController(ITokenService tokenService)
        {
            this.tokenService = tokenService;
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Login(string email, string password)
        {
            var token = await tokenService.GetToken("API", email, password);
            return Ok(token);
        }
    }
}
