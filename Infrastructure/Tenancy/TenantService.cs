using ABCShared.Library.Models.Requests.Tenancy;
using ABCShared.Library.Models.Responses.Tenancy;
using Application.Features.Tenancy;
using Finbuckle.MultiTenant;
using Finbuckle.MultiTenant.Abstractions;
using Infrastructure.Contexts;
using Mapster;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Tenancy;

public class TenantService(IMultiTenantStore<ABCSchoolTenantInfo> tenantStore, ApplicationDbSeeder dbSeeder, IServiceProvider serviceProvider) : ITenantService
{
    public async Task<string> ActivateAsync(string id)
    {
        var tenant = await tenantStore.TryGetByIdentifierAsync(id);
        tenant.IsActive = true;
        await tenantStore.TryUpdateAsync(tenant);
        return tenant.Identifier;
    }

    public async Task<string> CreateTenantAsync(CreateTenantRequest createTenant, CancellationToken ct)
    {
        var newTenant = new ABCSchoolTenantInfo
        {
            Id = createTenant.Identifier,
            Identifier = createTenant.Identifier,
            Name = createTenant.Name,
            FirstName = createTenant.FirstName,
            LastName = createTenant.LastName,
            Email = createTenant.Email,
            ConnectionString = createTenant.ConnectionString,
            ValidUpTo = createTenant.ValidUpTo
        };

        await tenantStore.TryAddAsync(newTenant);

        //Seeding tenant data
        using var scope = serviceProvider.CreateScope();
        serviceProvider.GetRequiredService<IMultiTenantContextSetter>()
            .MultiTenantContext = new MultiTenantContext<ABCSchoolTenantInfo>()
            {
                TenantInfo = newTenant
            };
        await scope.ServiceProvider.GetRequiredService<ApplicationDbSeeder>()
            .InitializeDatabaseAsync(ct);

        return newTenant.Identifier;
    }

    public async Task<string> DeactivateAsync(string id)
    {
        var tenant = await tenantStore.TryGetByIdentifierAsync(id);
        tenant.IsActive = false;
        await tenantStore.TryUpdateAsync(tenant);
        return tenant.Identifier;
    }

    public async Task<TenantResponse> GetTenantByIdAsync(string id)
    {
        var tenant = await tenantStore.TryGetByIdentifierAsync(id);
        return tenant.Adapt<TenantResponse>();
    }

    public async Task<List<TenantResponse>> GetTenantsAsync()
    {
        var tenants = await tenantStore.GetAllAsync();
        return tenants.Adapt<List<TenantResponse>>();
    }

    public async Task<string> UpdateSubscriptionAsync(UpdateTenantSubscriptionRequest request)
    {
        var tenant = await tenantStore.TryGetAsync(request.TenantId);
        tenant.ValidUpTo = request.NewExpiryDate;
        await tenantStore.TryUpdateAsync(tenant);
        return tenant.Identifier;
    }
}
