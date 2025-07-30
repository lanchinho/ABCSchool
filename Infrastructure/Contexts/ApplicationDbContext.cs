using Domain;
using Finbuckle.MultiTenant.Abstractions;
using Infrastructure.Tenancy;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Contexts;

public class ApplicationDbContext : BaseDbContext
{
    public ApplicationDbContext(
        IMultiTenantContextAccessor<ABCSchoolTenantInfo> tenantContextAccessor,
        DbContextOptions<ApplicationDbContext> options)
        : base(tenantContextAccessor, options)
    {
    }

    public DbSet<School> School => Set<School>();
}
