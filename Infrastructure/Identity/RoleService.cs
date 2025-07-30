using ABCShared.Library.Constants;
using ABCShared.Library.Models.Requests.Identity;
using Application.Exceptions;
using Application.Features.Identity.Roles;
using Finbuckle.MultiTenant.Abstractions;
using Infrastructure.Contexts;
using Infrastructure.Identity.Models;
using Infrastructure.Tenancy;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Identity;

public class RoleService : IRoleService
{
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _context;
    private readonly IMultiTenantContextAccessor<ABCSchoolTenantInfo> _tenantInfoContextAccessor;

    public RoleService(
        RoleManager<ApplicationRole> roleManager,
        UserManager<ApplicationUser> userManager,
        ApplicationDbContext context,
        IMultiTenantContextAccessor<ABCSchoolTenantInfo> tenantInfoContextAccessor)
    {
        _roleManager = roleManager;
        _userManager = userManager;
        _context = context;
        _tenantInfoContextAccessor = tenantInfoContextAccessor;
    }

    public async Task<string> CreateAsync(CreateRoleRequest request)
    {
        var newRole = new ApplicationRole
        {
            Name = request.Name,
            Description = request.Description
        };

        var result = await _roleManager.CreateAsync(newRole);
        if (!result.Succeeded)
            throw new IdentityException(IdentityHelper.GetIdentityResultErrorDescriptions(result));

        return newRole.Name;
    }

    public async Task<string> DeleteAsync(string id)
    {
        var roleToDelete = await _roleManager.FindByIdAsync(id)
            ?? throw new NotFoundException(["Role does not exist."]);

        if (RoleConstants.IsDefaultRole(roleToDelete.Name))
            throw new ConflictException([$"Not allowed to delete '{roleToDelete.Name}' role."]);

        if ((await _userManager.GetUsersInRoleAsync(roleToDelete.Name)).Count > 0)
            throw new ConflictException([$"Not allowed to delete '{roleToDelete.Name}' role as is currently assigned to user."]);

        var result = await _roleManager.DeleteAsync(roleToDelete);
        if (!result.Succeeded)
            throw new IdentityException(IdentityHelper.GetIdentityResultErrorDescriptions(result));

        return roleToDelete.Name;
    }

    public async Task<bool> DoesItExistsAsync(string name)
        => await _roleManager.RoleExistsAsync(name);

    public async Task<RoleResponse> GetRoleByIdAsync(string id, CancellationToken ct)
    {
        var roleInDb = await _context.Roles
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, ct);

        return roleInDb == null ? throw new NotFoundException(["Role does not exist."])
            : roleInDb.Adapt<RoleResponse>();
    }

    public async Task<List<RoleResponse>> GetAllAsync(CancellationToken ct)
    {
        var rolesInDb = await _roleManager.Roles.ToListAsync(ct);
        return rolesInDb.Adapt<List<RoleResponse>>();
    }

    public async Task<RoleResponse> GetRoleWithPermissions(string id, CancellationToken ct)
    {
        var role = await GetRoleByIdAsync(id, ct);
        role.Permissions = await _context.RoleClaims
            .Where(rc => rc.RoleId == id && rc.ClaimType == ClaimConstants.Permission)
            .Select(x => x.ClaimValue)
            .AsNoTracking()
            .ToListAsync(ct);

        return role;
    }

    public async Task<string> UpdateAsync(UpdateRoleRequest request)
    {
        var roleInDb = await _roleManager.FindByIdAsync(request.Id)
            ?? throw new NotFoundException(["Role does not exist."]);

        if (RoleConstants.IsDefaultRole(roleInDb.Name))
            throw new ConflictException([$"Changes are not allowwed on system role '{roleInDb.Name}'"]);

        roleInDb.Name = request.Name;
        roleInDb.NormalizedName = request.Name.ToUpperInvariant();
        roleInDb.Description = request.Description;

        var result = await _roleManager.UpdateAsync(roleInDb);
        if (!result.Succeeded)
            throw new IdentityException(IdentityHelper.GetIdentityResultErrorDescriptions(result));

        return roleInDb.Name;
    }

    public async Task<string> UpdatePermissions(UpdateRolePermissionsRequest request)
    {
        var roleInDb = await _roleManager.FindByIdAsync(request.RoleId)
            ?? throw new NotFoundException(["Role does not exist."]);

        if (roleInDb.Name == RoleConstants.Admin)
            throw new ConflictException([$"Not allow to change permissions for '{roleInDb.Name}'"]);

        if (_tenantInfoContextAccessor.MultiTenantContext.TenantInfo.Id != TenancyConstants.Root.Id)
            request.NewPermissions.RemoveAll(p => p.StartsWith("Permission.Tenants."));

        //Drop (delete) and lift(Create)
        var currentClaims = await _roleManager.GetClaimsAsync(roleInDb);
        foreach (var claim in currentClaims.Where(c => !request.NewPermissions.Any(p => p == c.Value)))
        {
            var result = await _roleManager.RemoveClaimAsync(roleInDb, claim);
            if (!result.Succeeded)
                throw new IdentityException(IdentityHelper.GetIdentityResultErrorDescriptions(result));
        }

        foreach (var newPermission in request.NewPermissions
                                             .Where(p => !currentClaims.Any(c => c.Value == p)))
        {
            await _context.RoleClaims.AddAsync(new ApplicationRoleClaim
            {
                RoleId = roleInDb.Id,
                ClaimType = ClaimConstants.Permission,
                ClaimValue = newPermission,
                Description = string.Empty,
                Group = string.Empty
            });
        }
        await _context.SaveChangesAsync();

        return "Permissions Updated Successfully.";
    }
}
