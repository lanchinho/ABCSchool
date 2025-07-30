using ABCShared.Library.Models.Requests.Users;
using ABCShared.Library.Wrappers;
using MediatR;

namespace Application.Features.Identity.Users.Commands;
public class CreateUserCommand : IRequest<IResponseWrapper>
{
    public CreateUserRequest CreateUserRequest { get; set; }
}

public class CreateUserCommandHandler(IUserService userService) : IRequestHandler<CreateUserCommand, IResponseWrapper>
{
    private readonly IUserService _userService = userService;

    public async Task<IResponseWrapper> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var response = await _userService.CreateAsync(request.CreateUserRequest);
        return await ResponseWrapper<string>.SuccessAsync(data: response, message: "User created successfully");
    }
}
