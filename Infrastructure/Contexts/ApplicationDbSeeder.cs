using ABCShared.Library.Constants;
using Finbuckle.MultiTenant.Abstractions;
using Infrastructure.Identity.Models;
using Infrastructure.Tenancy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Contexts;

public class ApplicationDbSeeder(
    IMultiTenantContextAccessor<ABCSchoolTenantInfo> tenantInfoContextAcessor,
    RoleManager<ApplicationRole> roleManager,
    UserManager<ApplicationUser> userManager,
    ApplicationDbContext applicationDbContext)
{
    private readonly IMultiTenantContextAccessor<ABCSchoolTenantInfo> _tenantInfoContextAcessor = tenantInfoContextAcessor;
    private readonly RoleManager<ApplicationRole> _roleManager = roleManager;
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly ApplicationDbContext _applicationDbContext = applicationDbContext;

    public async Task InitializeDatabaseAsync(CancellationToken cancellationToken)
    {
        //Apply pending migrations if there are any...
        if (_applicationDbContext.Database.GetAppliedMigrations().Any()
            && (await _applicationDbContext.Database.GetPendingMigrationsAsync(cancellationToken)).Any())
        {
            await _applicationDbContext.Database.MigrateAsync(cancellationToken);
        }

        //Seeding the DB
        if (await _applicationDbContext.Database.CanConnectAsync(cancellationToken))
        {
            //Default Roles > Assign permissions/claims
            await InitializeDefaultRolesAsync(cancellationToken);
            //Users
            await InitializeAdminUserAsync();

        }
    }

    private async Task InitializeDefaultRolesAsync(CancellationToken ct)
    {
        foreach (var roleName in RoleConstants.DefaultRoles)
        {
            if (await _roleManager.Roles.FirstOrDefaultAsync(role => role.Name == roleName, ct) is not ApplicationRole incomingRole)
            {
                incomingRole = new ApplicationRole()
                {
                    Name = roleName,
                    Description = $"{roleName} Role"
                };

                await _roleManager.CreateAsync(incomingRole);
            }

            //Assign permissions
            if (roleName == RoleConstants.Basic)
            {
                await AssignPermissionsToRoleAsync(SchoolPermissions.Basic, incomingRole, ct);
            }
            else
            {
                await AssignPermissionsToRoleAsync(SchoolPermissions.Admin, incomingRole, ct);
                if (_tenantInfoContextAcessor.MultiTenantContext.TenantInfo.Id == TenancyConstants.Root.Id)
                    await AssignPermissionsToRoleAsync(SchoolPermissions.Root, incomingRole, ct);
            }
        }
    }

    private async Task AssignPermissionsToRoleAsync(
        IReadOnlyList<SchoolPermission> rolePermissions,
        ApplicationRole role, CancellationToken ct)
    {
        var currentClaims = await _roleManager.GetClaimsAsync(role);

        foreach (var rolePermission in rolePermissions)
        {
            if (!currentClaims.Any(c => c.Type == ClaimConstants.Permission && c.Value == rolePermission.Name))
            {
                await _applicationDbContext.RoleClaims.AddAsync(new ApplicationRoleClaim
                {
                    RoleId = role.Id,
                    ClaimType = ClaimConstants.Permission,
                    ClaimValue = rolePermission.Name,
                    Description = rolePermission.Description,
                    Group = rolePermission.Group
                }, ct);

                await _applicationDbContext.SaveChangesAsync(ct);
            }
        }

    }

    private async Task InitializeAdminUserAsync()
    {
        if (string.IsNullOrWhiteSpace(_tenantInfoContextAcessor.MultiTenantContext.TenantInfo.Email)) return;

        if (await _userManager.Users
            .FirstOrDefaultAsync(user => user.Email == _tenantInfoContextAcessor.MultiTenantContext.TenantInfo.Email)
            is not ApplicationUser incomingUser)
        {
            incomingUser = new ApplicationUser
            {
                FirstName = _tenantInfoContextAcessor.MultiTenantContext.TenantInfo.FirstName ?? TenancyConstants.FirstName,
                LastName = _tenantInfoContextAcessor.MultiTenantContext.TenantInfo.LastName ?? TenancyConstants.LastName,
                Email = _tenantInfoContextAcessor.MultiTenantContext.TenantInfo.Email,
                UserName = _tenantInfoContextAcessor.MultiTenantContext.TenantInfo.Email,
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                NormalizedEmail = _tenantInfoContextAcessor.MultiTenantContext.TenantInfo.Email.ToUpperInvariant(),
                NormalizedUserName = _tenantInfoContextAcessor.MultiTenantContext.TenantInfo.Email.ToUpperInvariant(),
                IsActive = true
            };

            var passwordHash = new PasswordHasher<ApplicationUser>();
            incomingUser.PasswordHash = passwordHash.HashPassword(incomingUser, TenancyConstants.DefaultPassword);
            await _userManager.CreateAsync(incomingUser);
        }

        if (!await _userManager.IsInRoleAsync(incomingUser, RoleConstants.Admin))
            await _userManager.AddToRoleAsync(incomingUser, RoleConstants.Admin);
    }
}
