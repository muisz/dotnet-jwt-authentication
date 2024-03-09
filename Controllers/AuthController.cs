using JwtAuthTutorial.Data;
using JwtAuthTutorial.Services.AuthService;
using JwtAuthTutorial.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace JwtAuthTutorial.Controllers
{
    [Route("/api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _service;

        public AuthController(IAuthService service)
        {
            _service = service;
        }

        [HttpPost("register")]
        public ActionResult<AuthResponse> PostRegister(AuthRequest payload)
        {
            try
            {
                User user = _service.Register(payload);
                JwtToken token = _service.GenerateAuthToken(user);
                AuthResponse response = new AuthResponse
                {
                    Username = user.Username,
                    Token = token,
                };
                return Ok(response);
            }
            catch (Exception error)
            {
                return BadRequest(error.Message);
            }
        }

        [HttpPost("login")]
        public ActionResult<AuthResponse> PostLogin(AuthRequest payload)
        {
            try
            {
                User user = _service.Login(payload);
                JwtToken token = _service.GenerateAuthToken(user);
                AuthResponse response = new AuthResponse
                {
                    Username = user.Username,
                    Token = token,
                };
                return Ok(response);
            }
            catch (Exception error)
            {
                return BadRequest(error.Message);
            }
        }

        [HttpGet("me")]
        [Authorize]
        public ActionResult<AuthMeResponse> GetMe()
        {
            return Ok(User?.Identity.Name);
        }
    }
}