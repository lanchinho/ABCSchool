using ABCShared.Library.Models.Requests.Schools;
using ABCShared.Library.Wrappers;
using Application.Pipelines.Marker;
using Domain;
using Mapster;
using MediatR;

namespace Application.Features.Schools.Commands;

public class CreateSchoolCommand : IRequest<IResponseWrapper>, IValidateMe
{
    public CreateSchoolRequest CreateSchool { get; set; }
}

public class CreateSchoolCommandHandler(ISchoolService schoolService) : IRequestHandler<CreateSchoolCommand, IResponseWrapper>
{
    private readonly ISchoolService _schoolService = schoolService;

    public async Task<IResponseWrapper> Handle(CreateSchoolCommand request, CancellationToken cancellationToken)
    {
        var school = request.CreateSchool.Adapt<School>();
        var schoolId = await _schoolService.CreateAsync(school);
        return await ResponseWrapperForStruct<int>.SuccessAsync(data: schoolId, "School created successfully.");
    }
}
