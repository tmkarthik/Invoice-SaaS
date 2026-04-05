using System.Security.Claims;
using InvoiceSaaS.Application.Interfaces;
using Microsoft.AspNetCore.Http;

namespace InvoiceSaaS.Infrastructure.Authentication;

public sealed class UserContext(IHttpContextAccessor httpContextAccessor) : IUserContext
{
    public string? UserId => httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
    public string? UserName => httpContextAccessor.HttpContext?.User?.Identity?.Name;
    public bool IsAuthenticated => httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
}
