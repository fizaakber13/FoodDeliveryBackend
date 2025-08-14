using FoodDeliveryBackend.DTOs.Requests.Auth;
using FoodDeliveryBackend.DTOs.Responses.Auth;
using FoodDeliveryBackend.Services;
using FoodDeliveryBackend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FoodDeliveryBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ITokenService _tokenService;
        private readonly IUserService _userService;

        public AuthController(ITokenService tokenService, IUserService userService)
        {
            _tokenService = tokenService;
            _userService = userService;
        }

        [AllowAnonymous]
        [HttpPost("send-otp")]
        public async Task<IActionResult> SendOtp([FromBody] OtpSendRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email))
                return BadRequest("Email is required.");

            var userExists = await _userService.SendOtpAsync(request.Email);
            if (!userExists)
            {
                return NotFound(new { Message = "User not found." });
            }

            return Ok(new { Message = "OTP sent successfully." });
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Name))
                return BadRequest("Email and Name are required.");

            var userExists = await _userService.RegisterSendOtpAsync(request.Email, request.Name);
            if (!userExists)
            {
                return Conflict(new { Message = "User already exists." });
            }

            return Ok(new { Message = "OTP sent successfully." });
        }

        [AllowAnonymous]
        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp([FromBody] FoodDeliveryBackend.DTOs.Requests.Auth.OtpVerification request)
        {
            var result = await _userService.VerifyOtpAsync(request.Email, request.Otp, request.Name);
            if (result == null)
            {
                return BadRequest("Invalid OTP or email.");
            }
            return Ok(result);
        }

        [HttpPost("refresh")]
        [Authorize]
        public async Task<IActionResult> Refresh(TokenResponse tokenResponse)
        {
            var principal = _tokenService.GetPrincipalFromExpiredToken(tokenResponse.AccessToken);
            var userIdClaim = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
            {
                return BadRequest("Invalid token: User ID claim missing or invalid.");
            }
            var user = await _userService.GetUserByIdAsync(userId);

            if (user == null || user.RefreshToken != tokenResponse.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                return BadRequest("Invalid client request");
            }

            var newAccessToken = _tokenService.GenerateAccessToken(principal.Claims);
            var newRefreshToken = _tokenService.GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _userService.UpdateUserRefreshTokenAsync(user);

            return new ObjectResult(new { accessToken = newAccessToken, refreshToken = newRefreshToken });
        }
    }
}
