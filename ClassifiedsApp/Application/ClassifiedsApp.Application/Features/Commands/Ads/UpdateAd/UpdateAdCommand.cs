using ClassifiedsApp.Application.Common.Results;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace ClassifiedsApp.Application.Features.Commands.Ads.UpdateAd;

//public class UpdateAdCommand : IRequest<Result>
//{
//	public Guid Id { get; set; }
//	public string Description { get; set; }
//	public decimal Price { get; set; }
//	public bool IsNew { get; set; }
//}

public class UpdateAdCommand : IRequest<Result>
{
	public Guid Id { get; set; }
	public string Description { get; set; }
	public decimal Price { get; set; }
	public bool IsNew { get; set; }

	// Images yönetimi için
	public IList<Guid>? ExistingImages { get; set; }
	public IList<IFormFile>? NewImages { get; set; }
	public IList<Guid>? ImagesToDelete { get; set; }

}
