using ClassifiedsApp.Application.Common.Results;
using MediatR;

namespace ClassifiedsApp.Application.Features.Queries.Locations.GetAllLocations;

public class GetAllLocationsQuery : IRequest<Result<GetAllLocationsQueryResponse>>
{
}
