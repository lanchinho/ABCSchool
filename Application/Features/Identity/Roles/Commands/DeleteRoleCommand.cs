﻿using ABCShared.Library.Wrappers;
using MediatR;

namespace Application.Features.Identity.Roles.Commands;

public class DeleteRoleCommand : IRequest<IResponseWrapper>
{
    public string RoleID { get; set; }
}

public class DeleteRoleCommandHandler(IRoleService roleService) : IRequestHandler<DeleteRoleCommand, IResponseWrapper>
{
    private readonly IRoleService _roleService = roleService;

    public async Task<IResponseWrapper> Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
    {
        var deletedRole = await _roleService.DeleteAsync(request.RoleID);
        return await ResponseWrapper<string>.SuccessAsync($"Role '{deletedRole}' was removed successfully.");
    }
}
