using ABCShared.Library.Models.Requests.Users;
using ABCShared.Library.Wrappers;
using MediatR;

namespace Application.Features.Identity.Users.Commands;
public class ChangePasswordCommand : IRequest<IResponseWrapper>
{
    public ChangePasswordRequest ChangePassword { get; set; }
}

public class ChangePasswordCommandHandler(IUserService userService)
    : IRequestHandler<ChangePasswordCommand, IResponseWrapper>
{
    private IUserService _userService = userService;

    public async Task<IResponseWrapper> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        var userId = await _userService.ChangePasswordAsync(request.ChangePassword);
        return await ResponseWrapper<string>.SuccessAsync(userId, "User password was changed successfully");
    }
}
