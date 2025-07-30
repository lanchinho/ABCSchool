using ABCShared.Library.Wrappers;
using Mapster;
using MediatR;

namespace Application.Features.Schools.Queries;

public class GetSchoolByNameQuery : IRequest<IResponseWrapper>
{
    public string Name { get; set; }
}

public class GetSchoolByNameQueryHandler : IRequestHandler<GetSchoolByNameQuery, IResponseWrapper>
{
    private readonly ISchoolService _schoolService;

    public GetSchoolByNameQueryHandler(ISchoolService schoolService)
    {
        _schoolService = schoolService;
    }

    public async Task<IResponseWrapper> Handle(GetSchoolByNameQuery request, CancellationToken cancellationToken)
    {
        var school = await _schoolService.GetByNameAsync(request.Name);
        if (school is not null)
            return await ResponseWrapper<SchoolResponse>.SuccessAsync(data: school.Adapt<SchoolResponse>(), "");

        return await ResponseWrapper<string>.FailAsync("School does not exist");
    }
}

