using System.ComponentModel.DataAnnotations;
using InvoiceSaaS.Application.Common.Models;
using InvoiceSaaS.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InvoiceSaaS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class AuthController(IAuthService authService) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var result = await authService.RegisterAsync(request.TenantId, request.CompanyId, request.Email, request.FullName, request.Password);
        return Ok(ApiResponse.SuccessResponse(result, "User registered successfully"));
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var result = await authService.LoginAsync(request.Email, request.Password);
        return Ok(ApiResponse.SuccessResponse(result, "Login successful"));
    }

    [AllowAnonymous]
    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshRequest request)
    {
        var result = await authService.RefreshTokenAsync(request.RefreshToken);
        return Ok(ApiResponse.SuccessResponse(result, "Token refreshed successfully"));
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        var userIdString = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (Guid.TryParse(userIdString, out var userId))
        {
            await authService.LogoutAsync(userId);
            return Ok(ApiResponse.SuccessResponse("Logged out successfully"));
        }
        return BadRequest(ApiResponse.Failure("Unable to extract user ID"));
    }
}

public record RegisterRequest([Required] Guid TenantId, [Required] Guid CompanyId, [Required] [EmailAddress] string Email, [Required] string FullName, [Required] string Password);
public record LoginRequest([Required] [EmailAddress] string Email, [Required] string Password);
public record RefreshRequest([Required] string RefreshToken);
