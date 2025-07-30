using ABCShared.Library.Models.Requests.Tenancy;
using ABCShared.Library.Models.Responses.Tenancy;

namespace Application.Features.Tenancy;

public interface ITenantService
{
    Task<string> CreateTenantAsync(CreateTenantRequest createTenant, CancellationToken ct);
    Task<string> ActivateAsync(string id);
    Task<string> DeactivateAsync(string id);
    Task<string> UpdateSubscriptionAsync(UpdateTenantSubscriptionRequest request);
    Task<List<TenantResponse>> GetTenantsAsync();
    Task<TenantResponse> GetTenantByIdAsync(string id);
}
