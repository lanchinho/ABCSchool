using ABCShared.Library.Models.Requests.Identity;

namespace Application.Features.Identity.Roles;

public interface IRoleService
{
    Task<string> CreateAsync(CreateRoleRequest request);
    Task<string> UpdateAsync(UpdateRoleRequest request);
    Task<string> DeleteAsync(string id);
    Task<string> UpdatePermissions(UpdateRolePermissionsRequest request);
    Task<bool> DoesItExistsAsync(string name);
    Task<List<RoleResponse>> GetAllAsync(CancellationToken ct);
    Task<RoleResponse> GetRoleByIdAsync(string id, CancellationToken ct);
    Task<RoleResponse> GetRoleWithPermissions(string id, CancellationToken ct);
}
