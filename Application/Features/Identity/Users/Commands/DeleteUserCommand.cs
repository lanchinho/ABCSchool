﻿using ABCShared.Library.Wrappers;
using MediatR;

namespace Application.Features.Identity.Users.Commands;
public class DeleteUserCommand : IRequest<IResponseWrapper>
{
    public string UserId { get; set; }
}

public class DeleteUserCommandHandler(IUserService userService)
    : IRequestHandler<DeleteUserCommand, IResponseWrapper>
{
    private readonly IUserService _userService = userService;
    public async Task<IResponseWrapper> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var userId = await _userService.DeleteAsync(request.UserId);
        return await ResponseWrapper<string>.SuccessAsync(userId, "User deleted successfully");
    }
}
