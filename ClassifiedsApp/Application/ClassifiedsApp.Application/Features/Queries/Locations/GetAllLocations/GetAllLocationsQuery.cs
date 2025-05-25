using ClassifiedsApp.Application.Common.Results;
using ClassifiedsApp.Application.Interfaces.Services.Cache;
using MediatR;

namespace ClassifiedsApp.Application.Features.Queries.Locations.GetAllLocations;

public class GetAllLocationsQuery : IRequest<Result<GetAllLocationsQueryResponse>>
{
}
