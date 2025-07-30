﻿using ABCShared.Library.Wrappers;
using MediatR;

namespace Application.Features.Identity.Roles.Queries;

public class GetAllRolesQuery : IRequest<IResponseWrapper> { }

public class GetAllRolesQueryHandler(IRoleService roleService) : IRequestHandler<GetAllRolesQuery, IResponseWrapper>
{
    private readonly IRoleService _roleService = roleService;

    public async Task<IResponseWrapper> Handle(GetAllRolesQuery request, CancellationToken cancellationToken)
    {
        var roles = await _roleService.GetAllAsync(cancellationToken);
        return await ResponseWrapper<List<RoleResponse>>.SuccessAsync(data: roles, "");
    }
}

