using ClassifiedsApp.Application.Common.Results;
using ClassifiedsApp.Application.Dtos.Ads;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace ClassifiedsApp.Application.Features.Commands.Ads.UpdateAd;

public class UpdateAdCommand : IRequest<Result>
{
	public Guid Id { get; set; }
	public string Description { get; set; }
	public decimal Price { get; set; }
	public bool IsNew { get; set; }
	public Guid? LocationId { get; set; }
	public IList<Guid>? ExistingImages { get; set; }
	public IList<IFormFile>? NewImages { get; set; }
	public IList<Guid>? ImagesToDelete { get; set; }

	public Guid? CategoryId { get; set; }
	public Guid? MainCategoryId { get; set; }
	public string? SubCategoryValuesJson { get; set; }

	[NotMapped]
	public IList<CreateAdSubCategoryValueDto> SubCategoryValues =>
		string.IsNullOrWhiteSpace(SubCategoryValuesJson)
			? new List<CreateAdSubCategoryValueDto>()
			: JsonSerializer.Deserialize<IList<CreateAdSubCategoryValueDto>>(SubCategoryValuesJson)!;

}
