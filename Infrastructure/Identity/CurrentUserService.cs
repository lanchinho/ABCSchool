using ABCShared.Library.Identity;
using Application.Exceptions;
using Application.Features.Identity.Users;
using System.Security.Claims;

namespace Infrastructure.Identity;
public class CurrentUserService : ICurrentUserService
{
    private ClaimsPrincipal _principal;
    public string Name => _principal.Identity.Name;

    public IEnumerable<Claim> GetUserClaims()
    {
        return _principal.Claims;
    }

    public string GetUserEmail()
    {
        return IsAuthenticated() ? _principal.GetEmail() : string.Empty;
    }

    public string GetUserId()
    {
        return IsAuthenticated() ? _principal.GetUserId() : string.Empty;
    }

    public string GetUserTenant()
    {
        return IsAuthenticated() ? _principal.GetTenant() : string.Empty;
    }

    public bool IsAuthenticated()
        => _principal.Identity.IsAuthenticated;

    public bool IsInRole(string roleName)
        => _principal.IsInRole(roleName);

    public void SetCurrentUser(ClaimsPrincipal principal)
    {
        if (_principal is not null)
            throw new ConflictException(["Invalid operation on claim."]);

        _principal = principal;
    }
}
