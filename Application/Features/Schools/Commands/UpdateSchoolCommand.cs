using ABCShared.Library.Models.Requests.Schools;
using ABCShared.Library.Wrappers;
using Application.Pipelines.Marker;
using Domain;
using Mapster;
using MediatR;

namespace Application.Features.Schools.Commands;

public class UpdateSchoolCommand : IRequest<IResponseWrapper>, IValidateMe
{
    public UpdateSchoolRequest UpdateSchool { get; set; }
}

public class UpdateSchoolCommandHandler : IRequestHandler<UpdateSchoolCommand, IResponseWrapper>
{
    private readonly ISchoolService _schoolService;

    public UpdateSchoolCommandHandler(ISchoolService schoolService)
    {
        _schoolService = schoolService;
    }

    public async Task<IResponseWrapper> Handle(UpdateSchoolCommand request, CancellationToken cancellationToken)
    {
        var school = request.UpdateSchool.Adapt<School>();
        var schoolId = await _schoolService.UpdateAsync(school);
        return await ResponseWrapperForStruct<int>.SuccessAsync(data: schoolId, "School update successfully");
    }
}
