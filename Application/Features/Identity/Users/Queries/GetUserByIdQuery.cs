﻿using ABCShared.Library.Models.Responses.Users;
using ABCShared.Library.Wrappers;
using Mapster;
using MediatR;

namespace Application.Features.Identity.Users.Queries;
public class GetUserByIdQuery : IRequest<IResponseWrapper>
{
    public string UserId { get; set; }
}

public class GetUserByIdQueryHandler(IUserService userService)
    : IRequestHandler<GetUserByIdQuery, IResponseWrapper>
{
    private readonly IUserService _userService = userService;

    public async Task<IResponseWrapper> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _userService.GetByIdAsync(request.UserId, cancellationToken);
        return await ResponseWrapper<UserResponse>
            .SuccessAsync(user.Adapt<UserResponse>(), "");
    }
}