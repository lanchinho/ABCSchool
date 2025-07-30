using ABCShared.Library.Models.Requests.Users;
using ABCShared.Library.Wrappers;
using MediatR;

namespace Application.Features.Identity.Users.Commands;
public class UpdateUserRolesCommand : IRequest<IResponseWrapper>
{
    public string UserId { get; set; }
    public UserRolesRequest UserRoles { get; set; }
}

public class UpdateUserRolesCommandHandler(IUserService userService)
    : IRequestHandler<UpdateUserRolesCommand, IResponseWrapper>
{
    private readonly IUserService _userService = userService;

    public async Task<IResponseWrapper> Handle(UpdateUserRolesCommand request, CancellationToken cancellationToken)
    {
        var userId = await _userService.AssignRolesAsync(request.UserId, request.UserRoles);
        return await ResponseWrapper<string>.SuccessAsync(userId, "User roles were updated succesfully");
    }
}
