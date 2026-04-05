namespace InvoiceSaaS.Application.Interfaces;

public interface IUserContext
{
    string? UserId { get; }
    string? UserName { get; }
    bool IsAuthenticated { get; }
}
