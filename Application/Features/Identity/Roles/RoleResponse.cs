namespace Application.Features.Identity.Roles;

public class RoleResponse
{
    public string Id { get; set; }
    public string Namme { get; set; }
    public string Description { get; set; }
    public List<string> Permissions { get; set; } = [];
}
