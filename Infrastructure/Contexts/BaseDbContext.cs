using Finbuckle.MultiTenant.Abstractions;
using Finbuckle.MultiTenant.EntityFrameworkCore;
using Infrastructure.Identity.Models;
using Infrastructure.Tenancy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Infrastructure.Contexts;

/// <summary>
/// DbContext do "subsistema" Identity...
/// </summary>
public abstract class BaseDbContext :
    MultiTenantIdentityDbContext<
        ApplicationUser,
        ApplicationRole,
        string,
        IdentityUserClaim<string>,
        IdentityUserRole<string>,
        IdentityUserLogin<string>,
        ApplicationRoleClaim,
        IdentityUserToken<string>>
{
    private new ABCSchoolTenantInfo TenantInfo { get; set; }

    protected BaseDbContext(IMultiTenantContextAccessor<ABCSchoolTenantInfo> tenantContextAccessor, DbContextOptions options)
        : base(tenantContextAccessor, options)
    {
        TenantInfo = tenantContextAccessor.MultiTenantContext.TenantInfo;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        if (!string.IsNullOrWhiteSpace(TenantInfo?.ConnectionString))
        {
            optionsBuilder.UseSqlServer(TenantInfo.ConnectionString, options =>
            {
                options.MigrationsAssembly(Assembly.GetExecutingAssembly().FullName);
            });
        }
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }
}
