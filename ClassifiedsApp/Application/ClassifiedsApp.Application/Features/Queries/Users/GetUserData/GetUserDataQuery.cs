using ClassifiedsApp.Application.Common.Results;
using ClassifiedsApp.Application.Features.Queries.Common;
using MediatR;

namespace ClassifiedsApp.Application.Features.Queries.Users.GetUserData;

public class GetUserDataQuery : IRequest<Result<GetUserDataQueryResponse>>
{
}
