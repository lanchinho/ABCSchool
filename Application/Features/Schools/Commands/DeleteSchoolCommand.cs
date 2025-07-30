using ABCShared.Library.Wrappers;
using Domain;
using MediatR;

namespace Application.Features.Schools.Commands;

public class DeleteSchoolCommand : IRequest<IResponseWrapper>
{
    public int SchoolId { get; set; }
}

public class DeleteSchoolCommandHandler : IRequestHandler<DeleteSchoolCommand, IResponseWrapper>
{
    private readonly ISchoolService _schoolService;

    public DeleteSchoolCommandHandler(ISchoolService schoolService)
    {
        _schoolService = schoolService;
    }

    public async Task<IResponseWrapper> Handle(DeleteSchoolCommand request, CancellationToken cancellationToken)
    {
        var schoolToRemove = new School
        {
            Id = request.SchoolId,
        };

        var schoolId = await _schoolService.DeleteAsync(schoolToRemove);
        return await ResponseWrapperForStruct<int>.SuccessAsync(data: schoolId, "School removed successfully");
    }
}
