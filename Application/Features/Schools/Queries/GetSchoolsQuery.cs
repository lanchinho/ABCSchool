using ABCShared.Library.Wrappers;
using Mapster;
using MediatR;

namespace Application.Features.Schools.Queries;

public class GetSchoolsQuery : IRequest<IResponseWrapper> { }

public class GetSchoolsQueryHandler : IRequestHandler<GetSchoolsQuery, IResponseWrapper>
{
    private readonly ISchoolService _schoolService;

    public GetSchoolsQueryHandler(ISchoolService schoolService)
    {
        _schoolService = schoolService;
    }

    public async Task<IResponseWrapper> Handle(GetSchoolsQuery request, CancellationToken cancellationToken)
    {
        var schools = await _schoolService.GetAllsync();
        if (schools == null || schools.Count == 0)
            return await ResponseWrapper<List<SchoolResponse>>.FailAsync("There are no schools registered");

        return await ResponseWrapper<List<SchoolResponse>>
            .SuccessAsync(data: schools.Adapt<List<SchoolResponse>>(), "");
    }
}


