using Microsoft.AspNetCore.Mvc;
using SmartInvoice.Shared.DTOs;

namespace SmartInvoice.Auth.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        [HttpGet("health")]
        public IActionResult HealthCheck()
        {
            return Ok(new ApiResponse<HealthResponse>
            {
                Success = true,
                Message = "Auth service is healthy",
                Data = new HealthResponse
                {
                    Service = "Authentication Service",
                    Status = "Healthy",
                    Timestamp = DateTime.UtcNow,
                    Details = new Dictionary<string, string>
                    {
                        { "Port", "5001" },
                        { "Version", "1.0.0" },
                        { "Features", "JWT, User Management" }
                    }
                }
            });
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            // TODO: Implement actual login logic
            return Ok(new ApiResponse<LoginResponse>
            {
                Success = true,
                Message = "Login endpoint - to be implemented",
                Data = new LoginResponse
                {
                    Token = "sample-jwt-token",
                    ExpiresAt = DateTime.UtcNow.AddHours(2)
                }
            });
        }

        // Request/Response classes (move to Shared later)
        public class LoginRequest
        {
            public string Email { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
        }

        public class LoginResponse
        {
            public string Token { get; set; } = string.Empty;
            public DateTime ExpiresAt { get; set; }
        }
    }
}